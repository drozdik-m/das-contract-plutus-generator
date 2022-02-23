using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;
using DasContract.Blockchain.Plutus.Code.Types.Determined;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using DasContract.Blockchain.Plutus.Data.Processes.Process;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Activities;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Events;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Gateways;
using DasContract.Blockchain.Plutus.Data.Processes.Process.MultiInstances;

namespace DasContract.Blockchain.Plutus.Transitions.NonTx
{
    public class NonTxTransitionVisitor : TransitionVisitor
    {
        public NonTxTransitionVisitor()
            : base()
        {

        }

        public NonTxTransitionVisitor(INamable subprocess)
            : base(subprocess)
        {

        }

        /// <summary>
        /// Returns a comment that indicates transition direction
        /// </summary>
        /// <param name="sourceName">The name of the source</param>
        /// <param name="targetName">The name of the target</param>
        /// <returns></returns>
        IPlutusCode TransitionComment(string sourceName, string targetName)
        {
            return new PlutusCode(new IPlutusLine[]
            {
                new PlutusComment(0, $"{sourceName} -> {targetName}")
            });
        }

        /// <summary>
        /// Returns a comment that indicates transition direction with a stack-return note
        /// </summary>
        /// <param name="sourceName">The name of the source</param>
        /// <param name="targetName">The name of the target</param>
        /// <returns></returns>
        IPlutusCode TransitionCommentWithReturn(string sourceName, string targetName, string returnName)
        {
            return new PlutusCode(new IPlutusLine[]
            {
                new PlutusComment(0, $"{sourceName} -> {targetName} / return {returnName}")
            });
        }

        /// <summary>
        /// Returns parameter names for the current state
        /// </summary>
        /// <param name="currentStateName">The name of the current state</param>
        /// <returns></returns>
        IEnumerable<string> CurrentStateParams(string currentStateName)
        {
            return new string[]
            {
                "dat@ContractDatum{ contractState = " + currentStateName + " }"
            };
        }

        /// <summary>
        /// Snippet code that transitions into the subprocess of target call activity
        /// </summary>
        /// <param name="target">The call activity with the subprocess</param>
        /// <returns></returns>
        string CallTransitionSnippet(ContractCallActivity callActivity)
        {
            var futureName = FutureElementName(callActivity.CalledProcess.StartEvent, callActivity.CalledProcess);
            var returnName = FutureElementName(callActivity, Subprocess);

            var returnNamePushedState = returnName;
            if (returnName.Any(char.IsWhiteSpace))
                returnNamePushedState = $"({returnNamePushedState})";

            var result = TransitionFunctionSignature.Name + 
                $" $ pushState {returnNamePushedState} $ datum " + "{ " +
                $"contractState = {futureName}" +
                " }";

            return result;
        }

        /// <summary>
        /// Snippet code that transitions into the subprocess of target call activity
        /// </summary>
        /// <param name="source">The source process element</param>
        /// <param name="target">The call activity with the subprocess</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">The target activity was not a call activity</exception>
        string CallTransitionSnippet(ContractProcessElement target)
        {
            if (!(target is ContractCallActivity callActivity))
                throw new ArgumentException("Target element was not a ContractCallActivity");

            return CallTransitionSnippet(callActivity);   
        }

        /// <summary>
        /// Code generating the transition function for subprocess call
        /// </summary>
        /// <param name="source">Current source process element</param>
        /// <param name="callActivity">Call activity for a subprocess</param>
        /// <returns></returns>
        IPlutusCode CallTransition(ContractProcessElement source, ContractCallActivity callActivity)
        {
            var currentName = CurrentElementName(source, Subprocess);
            var futureName = FutureElementName(callActivity.CalledProcess.StartEvent, callActivity.CalledProcess);
            var returnName = FutureElementName(callActivity, Subprocess);

            //Transition to the subprocess
            var transitionFunction = new PlutusFunction(0, 
                TransitionFunctionSignature,
                CurrentStateParams(currentName),
                new IPlutusLine[]
                {
                    new PlutusRawLine(1, CallTransitionSnippet(callActivity)),
                });

            return TransitionCommentWithReturn(currentName, futureName, returnName)
                .Append(transitionFunction)
                .Append(PlutusLine.Empty);
        }

        /// <summary>
        /// Code generating the transition function for subprocess call
        /// </summary>
        /// <param name="source">Current source process element</param>
        /// <param name="callActivity">Call activity for a subprocess</param>
        /// <returns></returns>
        IPlutusCode CallTransition(ContractProcessElement source, ContractProcessElement target)
        {
            if (!(target is ContractCallActivity callActivity))
                throw new ArgumentException("Target element was not a ContractCallActivity");

            return CallTransition(source, callActivity);
        }

        /// <summary>
        /// Snippet code that transitions into another state
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        string SimpleStateTransitionSnippet(ContractProcessElement target)
        {
            var futureName = FutureElementName(target, Subprocess);
            return TransitionFunctionSignature.Name + " $ dat{ contractState = " + futureName + " }";
        }

        /// <summary>
        /// Simple transition that changes a state from a source to a target
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        IPlutusCode SimpleStateTransition(ContractProcessElement source, ContractProcessElement target)
        {
            var sourceName = CurrentElementName(source, Subprocess);
            var futureName = FutureElementName(target, Subprocess);

            var transitionFunction = new PlutusFunction(0, 
                TransitionFunctionSignature, 
                CurrentStateParams(sourceName),
                new IPlutusLine[]
                {
                        new PlutusRawLine(1, SimpleStateTransitionSnippet(target))
                });

            return TransitionComment(sourceName, futureName)
                .Append(transitionFunction)
                .Append(PlutusLine.Empty)
                .Append(target.Accept(this));
        }

        /// <summary>
        /// Snippet code that transitions into another state and executes a code
        /// </summary>
        /// <param name="source"></param>
        /// <param name="scriptActivity"></param>
        /// <returns></returns>
        string ScriptTransitionSnippet(ContractScriptActivity scriptActivity,  
            out IEnumerable<IPlutusLine> userDefinedTransition,
            string transformFunctionName = "userDefinedNewDatum")
        {
            var futureName = FutureElementName(scriptActivity, Subprocess);
            userDefinedTransition = ScriptTransitionUserDefinedSnippet(scriptActivity, transformFunctionName);
            return TransitionFunctionSignature.Name + " $ " + transformFunctionName + " dat{ contractState = " + futureName + " }";
        }

        IEnumerable<IPlutusLine> ScriptTransitionUserDefinedSnippet(
            ContractScriptActivity scriptActivity, 
            string transformFunctionName = "userDefinedNewDatum")
        {
            var userDefinedFuncSig = new PlutusFunctionSignature(2, transformFunctionName, new INamable[]
                {
                    PlutusContractDatum.Type,
                    PlutusContractDatum.Type,
                });

            IEnumerable<IPlutusLine> transitionCodeLines = new IPlutusLine[] { new PlutusRawLine(3, "datum") };
            if (scriptActivity.TransitionCodeLines.Count() > 0)
                transitionCodeLines = scriptActivity
                    .TransitionCodeLines
                    .Select(e => new PlutusRawLine(3, e));

            var userDefinedFunction = PlutusFunction.GetLinesOfCode(2, userDefinedFuncSig, new string[]
               {
                    "datum"
               }, transitionCodeLines);

            return userDefinedFunction
                .Prepend(userDefinedFuncSig);
        }

        /// <summary>
        /// Transition to a script 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        IPlutusCode ScriptTransition(ContractProcessElement source, ContractScriptActivity target)
        {
            var sourceName = CurrentElementName(source, Subprocess);
            var futureName = FutureElementName(target, Subprocess);

            var codeSnippet = ScriptTransitionSnippet(target, out IEnumerable<IPlutusLine> userDefinedTransition);

            var resultCode = new List<IPlutusLine>
                {
                        new PlutusRawLine(1, codeSnippet),
                        PlutusLine.Empty,
                        new PlutusRawLine(1, "where"),
                };
            resultCode.AddRange(userDefinedTransition);
            resultCode.Add(PlutusLine.Empty);

            var transitionFunction = new PlutusFunction(0,
                TransitionFunctionSignature,
                CurrentStateParams(sourceName),
                resultCode);

            return TransitionComment(sourceName, futureName)
                .Append(transitionFunction)
                .Append(PlutusLine.Empty)
                .Append(target.Accept(this));
        }

        IPlutusCode SingleOutputCommongTransition(ContractProcessElement source, ContractProcessElement target)
        {
            var typeVisitor = new TxTypeVisitor();
            var txType = target.Accept(typeVisitor);

            //Tx types
            if (txType == TxType.Implicit || txType == TxType.Tx)
                return target.Accept(this);

            //NonTx type
            else if (txType == TxType.NonTx)
            {
                //Target is an activity
                if (target is ContractActivity activity)
                {
                    //Target is sequential multi instance activity
                    if (activity.MultiInstance is ContractSequentialMultiInstance)
                        return SimpleStateTransition(source, target);

                    //Target is contract call activity
                    else if (activity is ContractCallActivity callActivity)
                        return CallTransition(source, callActivity);

                    //Target is contract script activity
                    else if (activity is ContractScriptActivity scriptActivity)
                        return ScriptTransition(source, scriptActivity);

                    //Other activities
                    else
                        return SimpleStateTransition(source, target);
                }

                //Other situations
                else
                    return SimpleStateTransition(source, target);
            }

            //Unhandled tx type
            else
                throw new Exception("Unknown TxType");
        }


        #region elementTransitions

        /// <inheritdoc/>
        public override IPlutusCode Visit(ContractExclusiveGateway element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            var targets = element.Outgoing;

            var guardLines = new List<IPlutusLine>();
            var whereLines = new List<IPlutusLine>();

 
            var i = 0;
            foreach (var targetConnection in targets)
            {
                var target = targetConnection.Target;
                var condition = targetConnection.Condition;

                var typeVisitor = new TxTypeVisitor();
                var txType = target.Accept(typeVisitor);

                //Tx types only
                if (txType != TxType.Tx)
                {
                    i++;
                    continue;
                }
                    
                //Target is an activity
                if (target is ContractActivity activity)
                {
                    //Target is sequential multi instance activity
                    if (activity.MultiInstance is ContractSequentialMultiInstance)
                        guardLines.Add(new PlutusRawLine(1, $"| {condition} = {SimpleStateTransitionSnippet(target)}"));

                    //Target is contract call activity
                    else if (activity is ContractCallActivity callActivity)
                        guardLines.Add(new PlutusRawLine(1, $"| {condition} = {CallTransitionSnippet(callActivity)}"));

                    //Target is contract script activity
                    else if (activity is ContractScriptActivity scriptActivity)
                    {
                        guardLines.Add(new PlutusRawLine(1, $"| {condition} = {ScriptTransitionSnippet(scriptActivity, out IEnumerable<IPlutusLine> lines, $"userDefinedNewDatum{i}")}"));
                        whereLines.AddRange(lines);
                    }
                        
                    //Other activities
                    else
                        guardLines.Add(new PlutusRawLine(1, $"| {condition} = {SimpleStateTransitionSnippet(target)}"));
                }

                //Other situations
                else
                    return SimpleStateTransition(element, target);

                i++;
            }

            var resultFunction = new PlutusFunction(0,
                TransitionFunctionSignature,
                CurrentStateParams(CurrentElementName(element, Subprocess)),
                guardLines
                    .Append(PlutusLine.Empty)
                    .Append(new PlutusRawLine(1, "where"))
                    .Concat(whereLines))
                    .Append(PlutusLine.Empty);

            foreach(var targetConnection in targets)
                resultFunction.Append(targetConnection.Target.Accept(this));

            return resultFunction;
        }

        /// <inheritdoc/>
        public override IPlutusCode Visit(ContractMergingExclusiveGateway element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            var target = element.Outgoing;
            return SingleOutputCommongTransition(element, target);
        }

        /// <inheritdoc/>
        public override IPlutusCode Visit(ContractCallActivity element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            //Loop transition
            IPlutusCode loopTransition = PlutusCode.Empty;
            if (element.MultiInstance is ContractSequentialMultiInstance)
            {
                var currentName = AddSubprocessPrefix(Subprocess, $"{element.Name} (ToLoop i)");
                var returnName = AddSubprocessPrefix(Subprocess, $"{element.Name} (toNextSeqMultiInstance i)");
                var targetName = AddSubprocessPrefix(element.CalledProcess, element.CalledProcess.StartEvent.Name);

                var codeSnippet = TransitionFunctionSignature.Name +
                    $" $ pushState ({returnName}) $ datum " +
                    "{ contractState = " + targetName + " }";

                loopTransition = TransitionCommentWithReturn(currentName, targetName, returnName)
                    .Append(new PlutusFunction(0,
                        TransitionFunctionSignature,
                        CurrentStateParams(currentName),
                        new IPlutusLine[]
                        {
                            new PlutusRawLine(1, codeSnippet),
                            PlutusLine.Empty,
                        }));
            }

            //Next transition
            var nextTransition = SingleOutputCommongTransition(element, element.Outgoing);

            return loopTransition
                .Append(nextTransition);
        }

        /// <inheritdoc/>
        public override IPlutusCode Visit(ContractUserActivity element)
        {
            // Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            var target = element.Outgoing;
            return SingleOutputCommongTransition(element, target);
        }

        /// <inheritdoc/>
        public override IPlutusCode Visit(ContractScriptActivity element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            //Loop transition
            IPlutusCode loopTransition = PlutusCode.Empty;
            if (element.MultiInstance is ContractSequentialMultiInstance)
            {
                var currentName = AddSubprocessPrefix(Subprocess, $"{element.Name} (ToLoop i)");
                var targetName = AddSubprocessPrefix(Subprocess, $"{element.Name} (toNextSeqMultiInstance i)");

                var codeSnippet = ScriptTransitionUserDefinedSnippet(element);

                var resultCode = new List<IPlutusLine>
                {
                        new PlutusRawLine(1, TransitionFunctionSignature.Name + " $ userDefinedNewDatum dat { contractState = " + targetName + " }"),
                        PlutusLine.Empty,
                        new PlutusRawLine(1, "where"),
                };
                resultCode.AddRange(codeSnippet);
                resultCode.Add(PlutusLine.Empty);

                loopTransition = TransitionComment(currentName, targetName)
                    .Append(new PlutusFunction(0,
                        TransitionFunctionSignature,
                        CurrentStateParams(currentName),
                        resultCode));
            }

            //Next transition
            var nextTransition = SingleOutputCommongTransition(element, element.Outgoing);

            return loopTransition
                .Append(nextTransition);
        }

        /// <inheritdoc/>
        public override IPlutusCode Visit(ContractTimerBoundaryEvent element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            var target = element.TimeOutDirection;
            return SingleOutputCommongTransition(element, target);
        }

        /// <inheritdoc/>
        public override IPlutusCode Visit(ContractStartEvent element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            var target = element.Outgoing;
            return SingleOutputCommongTransition(element, target);
        }
        #endregion

        public static PlutusFunctionSignature TransitionFunctionSignature { get; }
            = new PlutusFunctionSignature(0, "doNonTxTransition", new INamable[]
            {
                PlutusContractDatum.Type,
                PlutusContractDatum.Type,
            });
    }
}
