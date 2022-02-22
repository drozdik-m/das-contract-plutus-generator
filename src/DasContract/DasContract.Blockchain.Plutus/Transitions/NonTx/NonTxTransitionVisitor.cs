using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;
using DasContract.Blockchain.Plutus.Code.Types.Determined;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using DasContract.Blockchain.Plutus.Data.Processes.Process;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Activities;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Events;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Gateways;

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

            var result = $"$ pushState ({returnName}) $ datum " + "{ "
                + $"contractState = {futureName}" +
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

            //Transition to the subprocess
            var transitionFunction = new PlutusFunction(0, 
                TransitionFunctionSignature,
                CurrentStateParams(currentName),
                new IPlutusLine[]
                {
                    new PlutusRawLine(1, CallTransitionSnippet(callActivity)),
                });

            return TransitionComment(currentName, futureName)
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

        #region elementTransitions
        
        /// <inheritdoc/>
        public override IPlutusCode Visit(ContractExclusiveGateway element)
        {
            //Already visited
            /*if (!TryVisit(element))
                return PlutusCode.Empty;

            var typeVisitor = new TxTypeVisitor();
            var txType = element.Outgoing.Accept(typeVisitor);

            var currentName = CurrentElementName(element, Subprocess);

            //NonTx types
            if (txType == TxType.Implicit || txType == TxType.Tx)
                return PlutusCode.Empty;

            //Tx type
            else if (txType == TxType.NonTx)
            {
                var futureName = FutureElementName(element.Outgoing, Subprocess);

                var transitionComment = new PlutusComment(0, $"{currentName} -> {futureName}");
                var transitionFunction = new PlutusFunction(0, TransitionFunctionSignature, new string[]
                {
                    "dat@ContractDatum{ contractState = " + currentName + " }",
                }, new IPlutusLine[]
                {
                    new PlutusRawLine(1, TransitionFunctionSignature.Name + " $ datum{ contractState = " + futureName + " }")
                });

                return new PlutusCode(new IPlutusLine[] { transitionComment })
                    .Append(transitionFunction)
                    .Append(PlutusLine.Empty)
                    .Append(element.Outgoing.Accept(this));
            }

            //Call type
            else if (txType == TxType.Call)
                throw new NotImplementedException();

            //Unhandled type
            else
                throw new Exception("Unknown TxType");*/

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override IPlutusCode Visit(ContractMergingExclusiveGateway element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            var typeVisitor = new TxTypeVisitor();
            var txType = element.Outgoing.Accept(typeVisitor);

            var currentName = CurrentElementName(element, Subprocess);
            
            //NonTx types
            if (txType == TxType.Implicit || txType == TxType.Tx)
                return PlutusCode.Empty;

            //Tx type
            else if (txType == TxType.NonTx)
            {
                var futureName = FutureElementName(element.Outgoing, Subprocess);

                var transitionComment = new PlutusComment(0, $"{currentName} -> {futureName}");
                var transitionFunction = new PlutusFunction(0, TransitionFunctionSignature, new string[]
                {
                    "dat@ContractDatum{ contractState = " + currentName + " }",
                }, new IPlutusLine[]
                {
                    new PlutusRawLine(1, TransitionFunctionSignature.Name + " $ datum{ contractState = " + futureName + " }")
                });

                return new PlutusCode(new IPlutusLine[] { transitionComment })
                    .Append(transitionFunction)
                    .Append(PlutusLine.Empty)
                    .Append(element.Outgoing.Accept(this));
            }

            //Call type
            else if (txType == TxType.Call)
            {
                throw new NotImplementedException();
                
            }

            //Unhandled type
            else
                throw new Exception("Unknown TxType");
        }

        /// <inheritdoc/>
        public override IPlutusCode Visit(ContractCallActivity element)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override IPlutusCode Visit(ContractUserActivity element)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override IPlutusCode Visit(ContractScriptActivity element)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override IPlutusCode Visit(ContractTimerBoundaryEvent contractTimerBoundaryEvent)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override IPlutusCode Visit(ContractStartEvent element)
        {
            throw new NotImplementedException();
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
