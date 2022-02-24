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
    public class TxTransitionVisitor : TransitionVisitor
    {
        public TxTransitionVisitor()
            : base()
        {

        }

        public TxTransitionVisitor(INamable subprocess)
            : base(subprocess)
        {

        }

        /// <summary>
        /// Returns parameter names for the current state
        /// </summary>
        /// <param name="currentStateName">The name of the current state</param>
        /// <returns></returns>
        string CurrentStateMatching(string currentStateName, string redeemerName)
        {
            string redeemer = redeemerName;

            if (redeemerName != PlutusContractFinishedRedeemer.Type.Name &&
                redeemerName != PlutusTimeoutRedeemer.Type.Name)
                redeemer = $"red@({redeemerName} f)";


            return "(par, dat@ContractDatum { contractState = " + currentStateName
                + " }, v, " + redeemer + "))";
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
                $" $ pushState {returnNamePushedState} $ dat " + "{ " +
                $"contractState = {futureName}" +
                " }";

            return result;
        }

        /// <summary>
        /// Standard transition for most elements with single output
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /*IPlutusCode SingleOutputCommongTransition(ContractProcessElement source, ContractProcessElement target)
        {
            var typeVisitor = new TxTypeVisitor(!(Subprocess is null));
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
        }*/

        #region elementTransitions

        /// <inheritdoc/>
        public override IPlutusCode Visit(ContractExclusiveGateway element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override IPlutusCode Visit(ContractMergingExclusiveGateway element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override IPlutusCode Visit(ContractCallActivity element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override IPlutusCode Visit(ContractUserActivity element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override IPlutusCode Visit(ContractScriptActivity element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override IPlutusCode Visit(ContractTimerBoundaryEvent element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override IPlutusCode Visit(ContractStartEvent element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override IPlutusCode Visit(ContractEndEvent element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            throw new NotImplementedException();
        }
        #endregion

        public static PlutusFunctionSignature TransitionFunctionSignature { get; }
            = new PlutusFunctionSignature(0, "txTransition", new INamable[]
            {
                PlutusContractParam.Type,
                PlutusState.Type(PlutusContractDatum.Type),
                PlutusContractRedeemer.Type,
                PlutusMaybe.Type(PlutusTuple.Type(
                    PlutusUnspecifiedDataType.Type("TxConstraints Void Void"),
                    PlutusState.Type(PlutusContractDatum.Type))
                ),
            });
    }
}
