using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;
using DasContract.Blockchain.Plutus.Code.Types.Determined;
using DasContract.Blockchain.Plutus.Code.Types.Premade;
using DasContract.Blockchain.Plutus.Code.Types.Temporary;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using DasContract.Blockchain.Plutus.Data.Processes.Process;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Activities;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Events;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Gateways;
using DasContract.Blockchain.Plutus.Data.Processes.Process.MultiInstances;

namespace DasContract.Blockchain.Plutus.Transitions.NonTx
{
    /// <summary>
    /// A visitor the recursively traverses a process (not its subprocesses)
    ///  to put together a code for plutus contract non-transactional transitions. 
    /// </summary>
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
            if (callActivity?.CalledProcess?.StartEvent is null)
                throw new Exception($"Called process {callActivity?.Name} or its start event is null");

            var futureName = FutureElementName(callActivity.CalledProcess.StartEvent, callActivity.CalledProcess);
            var returnName = FutureElementName(callActivity, Subprocess);

            returnName = PlutusCode.ProperlyBracketed(returnName);

            var result = TransitionFunctionSignature.Name + 
                $" $ pushState {returnName} $ dat " + "{ " +
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
            if (callActivity?.CalledProcess?.StartEvent is null)
                throw new Exception($"Called process {callActivity?.Name} or its start event is null");

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

            return TransitionCommentWithReturn(0, currentName, futureName, returnName)
                .Append(transitionFunction)
                .Append(PlutusLine.Empty)
                .Append(callActivity.Accept(this));
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

            return TransitionComment(0, sourceName, futureName)
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

        /// <summary>
        /// Code for user defined method for script transitions (the code after "where")
        /// </summary>
        /// <param name="scriptActivity"></param>
        /// <param name="transformFunctionName"></param>
        /// <returns></returns>
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

            return TransitionComment(0, sourceName, futureName)
                .Append(transitionFunction)
                .Append(PlutusLine.Empty)
                .Append(target.Accept(this));
        }

        /// <summary>
        /// Standard transition for most elements with single output
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        IPlutusCode SingleOutputCommonTransition(ContractProcessElement source, ContractProcessElement target)
        {
            var typeVisitor = new TxTypeVisitor(!(Subprocess is null));
            var txType = target.Accept(typeVisitor);

            IPlutusCode result = PlutusCode.Empty;

            //Boundary events
            if (source is ContractActivity contractActivity)
            {
                foreach(var timerBoundaryEvent in contractActivity
                    .BoundaryEvents
                    .OfType<ContractTimerBoundaryEvent>())
                {
                    result = result.Append(timerBoundaryEvent.Accept(this));
                }
            }

            //Tx types
            if (txType == TxType.Implicit || txType == TxType.Tx)
                return target.Accept(this).Append(result);

            //NonTx type
            else if (txType == TxType.NonTx)
            {
                //Target is an activity
                if (target is ContractActivity activity)
                {
                    //Target is sequential multi instance activity
                    if (activity.MultiInstance is ContractSequentialMultiInstance)
                        return SimpleStateTransition(source, target).Append(result);

                    //Target is contract call activity
                    else if (activity is ContractCallActivity callActivity)
                        return CallTransition(source, callActivity).Append(result);

                    //Target is contract script activity
                    else if (activity is ContractScriptActivity scriptActivity)
                        return ScriptTransition(source, scriptActivity).Append(result);

                    //Other activities
                    else
                        return SimpleStateTransition(source, target).Append(result);
                }

                //Other situations
                else
                    return SimpleStateTransition(source, target).Append(result);
            }

            //Unhandled tx type
            else
                throw new Exception("Unknown TxType");
        }

        /// <summary>
        /// Transition that returns from a subprocess using the state stack
        /// </summary>
        /// <param name="endEvent"></param>
        /// <returns></returns>
        IPlutusCode ReturnFromSubprocessTransition(ContractEndEvent endEvent)
        {
            var sourceName = CurrentElementName(endEvent, Subprocess);

            var resultCode = new List<IPlutusLine>
                {
                        new PlutusRawLine(1, "let"),
                            new PlutusRawLine(2, "(newState, newDat) = popState dat"),
                        new PlutusRawLine(1, "in"),
                            new PlutusRawLine(2, TransitionFunctionSignature.Name +
                            " $ newDat { contractState = newState }"),
                        PlutusLine.Empty
                };

            var transitionFunction = new PlutusFunction(0,
                TransitionFunctionSignature,
                CurrentStateParams(sourceName),
                resultCode);

            return TransitionComment(0, sourceName, "/return/")
                .Append(transitionFunction);
        }

        #region elementTransitions

        /// <inheritdoc/>
        public override IPlutusCode Visit(ContractExclusiveGateway element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            var targets = element.Outgoing;

            var guardLines = new List<(string, string)>();
            var userDefinedLines = new List<IPlutusLine>();

            if (targets.Count == 0)
                throw new Exception("Exclusive Gateway has no outputs");

            var otherwiseTargets = targets.Where(e => e.Condition.Trim() == "otherwise");
            var remainingTargets = targets.Where(e => e.Condition.Trim() != "otherwise");
            targets = remainingTargets.Concat(otherwiseTargets).ToList();

            var i = 0;
            foreach (var targetConnection in targets)
            {
                var target = targetConnection.Target;
                var condition = targetConnection.Condition;

                var typeVisitor = new TxTypeVisitor(!(Subprocess is null));
                var txType = target.Accept(typeVisitor);

                //NonTx types only
                if (txType != TxType.NonTx)
                {
                    i++;
                    continue;
                }
                    
                //Target is an activity
                if (target is ContractActivity activity)
                {
                    //Target is sequential multi instance activity
                    if (activity.MultiInstance is ContractSequentialMultiInstance)
                        guardLines.Add((condition, SimpleStateTransitionSnippet(target)));

                    //Target is contract call activity
                    else if (activity is ContractCallActivity callActivity)
                        guardLines.Add((condition, CallTransitionSnippet(callActivity)));

                    //Target is contract script activity
                    else if (activity is ContractScriptActivity scriptActivity)
                    {
                        guardLines.Add((condition, ScriptTransitionSnippet(scriptActivity, out IEnumerable<IPlutusLine> lines, $"userDefinedNewDatum{i}")));
                        userDefinedLines.AddRange(lines);
                        userDefinedLines.Add(PlutusLine.Empty);
                    }
                        
                    //Other activities
                    else
                        guardLines.Add((condition, SimpleStateTransitionSnippet(target)));
                }

                //Other situations
                else
                    guardLines.Add((condition, SimpleStateTransitionSnippet(target)));

                i++;
            }



            IPlutusCode resultFunction = PlutusCode.Empty;
            if (guardLines.Count != 0)
            {
                var decitionFunctionSig = new PlutusFunctionSignature(2, "decitionFunction", new INamable[]
                {
                    PlutusContractDatum.Type,
                    PlutusContractDatum.Type,
                });
                IPlutusCode decitionFunction = new PlutusGuardFunction(2,
                    decitionFunctionSig,
                    new string[] { "datum" },
                    guardLines)
                    .Append(PlutusLine.Empty);

                var where = decitionFunction
                    .Prepend(decitionFunctionSig)
                    .Prepend(new PlutusRawLine(1, "where"));

                if (userDefinedLines.Count > 0)
                {
                    where = where.Append(PlutusLine.Empty);
                    where = where.Append(new PlutusCode(userDefinedLines));
                    where = where.Append(PlutusLine.Empty);
                }

                where = where.Prepend(PlutusLine.Empty);
                where = where.Prepend(new PlutusRawLine(1, $"{decitionFunctionSig.Name} dat"));

                resultFunction = new PlutusFunction(0,
                    TransitionFunctionSignature,
                    CurrentStateParams(CurrentElementName(element, Subprocess)),
                    Array.Empty<IPlutusLine>())
                .Append(where)
                .Prepend(TransitionComment(0, CurrentElementName(element), "/branch/"));
            }

            //Collect others
            foreach(var targetConnection in targets)
                resultFunction = resultFunction.Append(targetConnection.Target.Accept(this));

            return resultFunction;
        }

        /// <inheritdoc/>
        public override IPlutusCode Visit(ContractMergingExclusiveGateway element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            var target = element.Outgoing;
            return SingleOutputCommonTransition(element, target);
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
                    $" $ pushState ({returnName}) $ dat " +
                    "{ contractState = " + targetName + " }";

                loopTransition = TransitionCommentWithReturn(0, currentName, targetName, returnName)
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
            var nextTransition = SingleOutputCommonTransition(element, element.Outgoing);

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
            return SingleOutputCommonTransition(element, target);
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

                loopTransition = TransitionComment(0, currentName, targetName)
                    .Append(new PlutusFunction(0,
                        TransitionFunctionSignature,
                        CurrentStateParams(currentName),
                        resultCode));
            }

            //Next transition
            var nextTransition = SingleOutputCommonTransition(element, element.Outgoing);

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
            return SingleOutputCommonTransition(element, target);
        }

        /// <inheritdoc/>
        public override IPlutusCode Visit(ContractStartEvent element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            var target = element.Outgoing;
            return SingleOutputCommonTransition(element, target);
        }

        /// <inheritdoc/>
        public override IPlutusCode Visit(ContractEndEvent element)
        {
            //Main process end event
            if (Subprocess is null)
                return PlutusCode.Empty;

            //Subprocess end event
            return ReturnFromSubprocessTransition(element);
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
