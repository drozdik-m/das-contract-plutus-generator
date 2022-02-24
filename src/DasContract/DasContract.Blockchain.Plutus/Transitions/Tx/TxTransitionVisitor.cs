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
using DasContract.Blockchain.Plutus.Transitions.Tx;

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
        /// Returns formulated guard line for the transition
        /// </summary>
        /// <param name="expectedVal"></param>
        /// <param name="formValidation"></param>
        /// <param name="transitionCondition"></param>
        /// <returns></returns>
        IPlutusLine GuardLine(bool expectedVal = false, bool formValidation = false, string transitionCondition = "")
        {
            string userDefinedExpectedValue = $"{UserDefinedExpectedValueSignature.Name} par dat v";
            const string userDefinedFormValidation = "userDefinedFormValidation par dat red v";

            var conditions = new List<string>();

            if (expectedVal)
                conditions.Add(userDefinedExpectedValue);

            if (formValidation)
                conditions.Add(userDefinedFormValidation);

            if (!string.IsNullOrWhiteSpace(transitionCondition))
                conditions.Add(transitionCondition);

            if (conditions.Count == 0)
                return new PlutusRawLine(3, "->");

            return new PlutusRawLine(3, $"| {string.Join(" && ", conditions)} ->");
        }

        /// <summary>
        /// Constructs together the transition function return type (Just constrains state)
        /// </summary>
        /// <param name="constrains">IEnumerable of constraints</param>
        /// <param name="newDatumExp">New datum</param>
        /// <param name="newValueExp">New contract value</param>
        /// <returns></returns>
        IEnumerable<IPlutusLine> ReturningJust(IEnumerable<string> constrains, string newDatumExp, string newValueExp)
        {
            string constrainsLine = string.Join(" <> ", constrains);

            newDatumExp = PlutusCode.ProperlyBracketed(newDatumExp);
            newValueExp = PlutusCode.ProperlyBracketed(newValueExp);

            return new List<IPlutusLine>()
            {
                new PlutusRawLine(4, "Just ("),
                    new PlutusRawLine(5, constrainsLine),
                    new PlutusRawLine(5, "State " + newDatumExp),
                    new PlutusRawLine(5, "      " + newValueExp),
                new PlutusRawLine(4, "     )"),
            };
        }

        /// <summary>
        /// Generate where user defined statements for a user activity
        /// </summary>
        /// <param name="userActivity"></param>
        /// <param name="statements"></param>
        /// <returns></returns>
        IEnumerable<IPlutusLine> WhereStatements(ContractUserActivity userActivity, params TxUserDefinedStatement[] statements)
        {
            var result = new List<IPlutusLine>();

            var commonParams = new string[]
            {
                "param",
                "datum",
                "val"
            };

            void CommonLines(PlutusFunctionSignature sig, IEnumerable<string> parameters, IEnumerable<string> codeLines, string def)
            {
                result.Add(sig);
                var code = new List<IPlutusLine>();
                if (codeLines.Count() == 0)
                    code.Add(new PlutusRawLine(4, def));
                else
                {
                    code.AddRange(PlutusFunction.GetLinesOfCode(4,
                        sig,
                        parameters,
                        codeLines.Select(e => new PlutusRawLine(4, e))));
                    code.Add(PlutusLine.Empty);
                }
            }

            //Transition
            if (statements.Contains(TxUserDefinedStatement.UserDefinedTransition))
            {
                var signature = new PlutusFunctionSignature(4,
                    "userDefinedTransition",
                    new INamable[]
                    {
                        PlutusContractParam.Type,
                        PlutusContractDatum.Type,
                        PlutusValue.Type,
                        PlutusUserActivityForm.Type(userActivity),
                        PlutusContractDatum.Type,
                    });
                CommonLines(signature,
                    new string[]
                    {
                        "param",
                        "datum",
                        "val",
                        "form",
                    },
                    userActivity.TransitionCodeLines,
                    "datum");
            }

            //Expected value
            if (statements.Contains(TxUserDefinedStatement.UserDefinedExpectedValue))
            {
                CommonLines(UserDefinedNewValueSignature,
                    commonParams,
                    userActivity.NewValueCodeLines,
                    "val");
            }

            //New value
            if (statements.Contains(TxUserDefinedStatement.UserDefinedNewValue))
            {
                CommonLines(UserDefinedNewValueSignature,
                    commonParams,
                    userActivity.NewValueCodeLines,
                    "val");
            }
            
            //Constraints
            if (statements.Contains(TxUserDefinedStatement.UserDefinedConstraints))
            {
                CommonLines(UserDefinedConstraintsSignature,
                    commonParams,
                    userActivity.ContrainsCodeLines, 
                    "mempty");
            }

            return result;
        }

        /// <summary>
        /// Creates transition into the end event
        /// </summary>
        /// <param name="endEvent"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private IPlutusCode EndEventTransition(ContractProcessElement source, ContractEndEvent endEvent)
        {
            var currentName = CurrentElementName(source);
            var targetName = FutureElementName(endEvent);

            var matchLine = CurrentStateMatching(currentName, targetName);

            throw new NotImplementedException();
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

            var target = element.Outgoing;

            var typeVisitor = new TxTypeVisitor(!(Subprocess is null));
            var txType = target.Accept(typeVisitor);

            //NonTx types
            if (txType == TxType.Implicit || txType == TxType.NonTx)
                return target.Accept(this);

            //Tx type
            else if (txType == TxType.Tx)
            {
                //TODO end event target
                if (target is ContractEndEvent endEvent)
                    return EndEventTransition(element, endEvent);

                //TODO call activity traget

                //TODO call timeouted (non sequential) activity traget (target = timer boundary event)




                //Target is an activity
                /*if (target is ContractActivity activity)
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
                    return SimpleStateTransition(source, target);*/
            }

            //Unhandled tx type
            else
                throw new Exception("Unknown TxType");

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

        static PlutusFunctionSignature UserDefinedExpectedValueSignature { get; }
            = new PlutusFunctionSignature(4, "userDefinedExpectedValue", new INamable[]
            {
                PlutusContractParam.Type,
                PlutusContractDatum.Type,
                PlutusValue.Type,
                PlutusBool.Type
            });

        static PlutusFunctionSignature UserDefinedNewValueSignature { get; }
            = new PlutusFunctionSignature(4, "userDefinedNewValue", new INamable[]
            {
                PlutusContractParam.Type,
                PlutusContractDatum.Type,
                PlutusValue.Type,
                PlutusValue.Type
            });

        static PlutusFunctionSignature UserDefinedConstraintsSignature { get; }
            = new PlutusFunctionSignature(4, "userDefinedConstraints", new INamable[]
            {
                PlutusContractParam.Type,
                PlutusContractDatum.Type,
                PlutusValue.Type,
                PlutusUnspecifiedDataType.Type("TxConstraints Void Void")
            });
    }
}
