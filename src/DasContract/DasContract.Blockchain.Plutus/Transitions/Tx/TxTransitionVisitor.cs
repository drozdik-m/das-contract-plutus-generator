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
        /// Returns line for pattern matching of this state
        /// </summary>
        /// <param name="currentStateName"></param>
        /// <param name="redeemerName"></param>
        /// <returns></returns>
        IPlutusLine CurrentStateMatching(string currentStateName, string redeemerName)
        {
            string redeemer = redeemerName;

            if (redeemerName != PlutusContractFinishedRedeemer.Type.Name &&
                redeemerName != PlutusTimeoutRedeemer.Type.Name)
                redeemer = $"red@({redeemerName} f)";


            return new PlutusRawLine(2, "(par, dat@ContractDatum { contractState = " + currentStateName
                + " }, v, " + redeemer + "))");
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
            const string userDefinedFormValidation = "userDefinedFormValidation par dat v f";

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
        IPlutusCode ReturningJust(IEnumerable<string> constrains, string newDatumExp, string newValueExp)
        {
            string constrainsLine = string.Join(" <> ", constrains);
            if (constrains.Count() == 0)
                constrainsLine = "mempty";

            newDatumExp = PlutusCode.ProperlyBracketed(newDatumExp);
            newValueExp = PlutusCode.ProperlyBracketed(newValueExp);

            return new PlutusCode(
                    new IPlutusLine[]
                    {
                        new PlutusRawLine(4, "Just ("),
                            new PlutusRawLine(5, constrainsLine + ","),
                            new PlutusRawLine(5, "State " + newDatumExp),
                            new PlutusRawLine(5, "      " + newValueExp),
                        new PlutusRawLine(4, "     )"),
                    }
                );
        }

        /// <summary>
        /// Generate "where" user defined statements for a user activity
        /// </summary>
        /// <param name="userActivity"></param>
        /// <param name="statements"></param>
        /// <returns></returns>
        IPlutusCode WhereStatements(ContractUserActivity userActivity, params TxUserDefinedStatement[] statements)
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



                if (codeLines.Count() == 0)
                    codeLines = new List<string>
                    {
                        def
                    };

                result.AddRange(PlutusFunction.GetLinesOfCode(4,
                        sig,
                        parameters,
                        codeLines.Select(e => new PlutusRawLine(5, e))));
                result.Add(PlutusLine.Empty);
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
                CommonLines(UserDefinedExpectedValueSignature,
                    commonParams,
                    userActivity.ExpectedValueCodeLines,
                    "True");
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


            IPlutusCode resCode = new PlutusCode(result);

            if (result.Count > 0)
                resCode = resCode.Prepend(new PlutusRawLine(3, "where"));

            return resCode;
        }

        /// <summary>
        /// Creates transition into the final contract end event
        /// </summary>
        /// <param name="endEvent"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        IPlutusCode EndEventTransition(ContractProcessElement source, ContractEndEvent endEvent, string condition = "")
        {
            var currentName = CurrentElementName(source);
            var targetName = PlutusContractFinished.Type.Name;

            var comment = TransitionComment(2, currentName, targetName);
            var matchLine = CurrentStateMatching(currentName, PlutusContractFinishedRedeemer.Type.Name);
            var guardLine = GuardLine(transitionCondition: condition);
            var returningJust = ReturningJust(
                Array.Empty<string>(),
                "dat { contractState = " + targetName + " }",
                "lovelaceValueOf 0");

            return comment
                .Append(matchLine)
                .Append(guardLine)
                .Append(returningJust)
                .Append(PlutusLine.Empty);
        }

        /// <summary>
        ///  Creates transition into a user activity
        /// </summary>
        /// <param name="source"></param>
        /// <param name="userActivity"></param>
        /// <returns></returns>
        IPlutusCode UserActivityTransition(ContractProcessElement source, ContractUserActivity userActivity, string condition = "")
        {
            
            var currentName = CurrentElementName(source);

            IPlutusCode resultCode = PlutusCode.Empty;
            var boundaryTimerEvent = userActivity.BoundaryEvents.OfType<ContractTimerBoundaryEvent>().FirstOrDefault();
            

            //Regular transition
            {
                var targetName = FutureElementName(userActivity);

                var constraints = new List<string>
                {
                    UserDefinedConstraintsSignature.Name + " par dat v",
                    "TODO" //TODO user signature
                };

                if (!(boundaryTimerEvent is null))
                    constraints.Add($"Constraints.mustValidateIn (to $ {boundaryTimerEvent.TimerDefinition})");

                var comment = TransitionComment(2, currentName, targetName);
                var matchLine = CurrentStateMatching(currentName, PlutusUserActivityRedeemer.Type(userActivity).Name);
                var guardLine = GuardLine(expectedVal: true, formValidation: true, transitionCondition: condition);
                var returningJust = ReturningJust(
                    constraints,
                    NonTxTransitionVisitor.TransitionFunctionSignature.Name +
                        " $ (userDefinedTransition par dat v f) { contractState = " + targetName + " }",
                    UserDefinedNewValueSignature.Name + " par dat v");
                var whereStatements = WhereStatements(userActivity,
                    TxUserDefinedStatement.UserDefinedTransition,
                    TxUserDefinedStatement.UserDefinedExpectedValue,
                    TxUserDefinedStatement.UserDefinedNewValue,
                    TxUserDefinedStatement.UserDefinedConstraints);

                resultCode = resultCode
                    .Append(comment)
                    .Append(matchLine)
                    .Append(guardLine)
                    .Append(returningJust)
                    .Append(whereStatements)
                    .Append(PlutusLine.Empty);
            }

            //Timeout transition
            if (!(boundaryTimerEvent is null))
            {
                var targetName = FutureElementName(boundaryTimerEvent);

                var comment = TransitionCommentWithTimeout(2, currentName, targetName);
                var matchLine = CurrentStateMatching(currentName, PlutusTimeoutRedeemer.Type.Name);
                var guardLine = GuardLine(expectedVal: true, transitionCondition: condition);
                var returningJust = ReturningJust(
                    new string[]
                    {
                        $"Constraints.mustValidateIn (from $ 1 + {PlutusCode.ProperlyBracketed(boundaryTimerEvent.TimerDefinition)})"
                    },
                    NonTxTransitionVisitor.TransitionFunctionSignature.Name +
                        " $ datum { contractState = " + targetName + " }",
                    "v");
                var whereStatements = WhereStatements(userActivity,
                    TxUserDefinedStatement.UserDefinedExpectedValue);

                resultCode = resultCode
                    .Append(comment)
                    .Append(matchLine)
                    .Append(guardLine)
                    .Append(returningJust)
                    .Append(whereStatements)
                    .Append(PlutusLine.Empty);
            }

            resultCode = resultCode
                .Append(userActivity.Accept(this));

             if (!(boundaryTimerEvent is null))
                resultCode = resultCode
                    .Append(boundaryTimerEvent.Accept(this));

            return resultCode;
        }


        /// <summary>
        /// Standard tx transition for most elements with single output
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        IPlutusCode SingleOutputCommonTransition(ContractProcessElement source, ContractProcessElement target, string condition = "")
        {
            var typeVisitor = new TxTypeVisitor(!(Subprocess is null));
            var txType = target.Accept(typeVisitor);

            //NonTx types
            if (txType == TxType.Implicit || txType == TxType.NonTx)
                return target.Accept(this);

            //Tx type
            else if (txType == TxType.Tx)
            {
                //End event
                if (target is ContractEndEvent endEvent)
                    return EndEventTransition(source, endEvent, condition);

                //User activity
                if (target is ContractUserActivity userActivity)
                    return UserActivityTransition(source, userActivity, condition);

                //Unhandled tx element
                else
                    throw new Exception("Unhandled Tx element");
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

            IPlutusCode result = PlutusCode.Empty;
            foreach(var target in element.Outgoing)
            {
                result = result
                    .Append(SingleOutputCommonTransition(element, target.Target, target.Condition));
            }

            return result;
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

            var target = element.Outgoing;
            return SingleOutputCommonTransition(element, target);
        }

        /// <inheritdoc/>
        public override IPlutusCode Visit(ContractUserActivity element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            IPlutusCode resultCode = PlutusCode.Empty;

            //Sequential multi instance
            if (element.MultiInstance is ContractSequentialMultiInstance sequentialMultiInstance)
            {
                var boundaryTimerEvent = element.BoundaryEvents.OfType<ContractTimerBoundaryEvent>().FirstOrDefault();

                //Loop transition
                {
                    var currentName = AddSubprocessPrefix(Subprocess, $"{element.Name} (ToLoop i)");
                    var targetName = AddSubprocessPrefix(Subprocess, $"{element.Name} (toNextSeqMultiInstance i)");

                    var constraints = new List<string>
                    {
                        UserDefinedConstraintsSignature.Name + " par dat v",
                        "TODO" //TODO user signature
                    };

                    if (!(boundaryTimerEvent is null))
                        constraints.Add($"Constraints.mustValidateIn (to $ {boundaryTimerEvent.TimerDefinition})");

                    var comment = TransitionComment(2, currentName, targetName);
                    var matchLine = CurrentStateMatching(currentName, PlutusUserActivityRedeemer.Type(element).Name);
                    var guardLine = GuardLine(expectedVal: true, formValidation: true);
                    var returningJust = ReturningJust(
                        constraints,
                        NonTxTransitionVisitor.TransitionFunctionSignature.Name +
                            " $ (userDefinedTransition par dat v f) { contractState = " + targetName + " }",
                        UserDefinedNewValueSignature.Name + " par dat v");
                    var whereStatements = WhereStatements(element,
                        TxUserDefinedStatement.UserDefinedTransition,
                        TxUserDefinedStatement.UserDefinedExpectedValue,
                        TxUserDefinedStatement.UserDefinedNewValue,
                        TxUserDefinedStatement.UserDefinedConstraints);

                    resultCode = resultCode
                        .Append(comment)
                        .Append(matchLine)
                        .Append(guardLine)
                        .Append(returningJust)
                        .Append(whereStatements)
                        .Append(PlutusLine.Empty);
                }

                //Loop timeout transition
                if (!(boundaryTimerEvent is null))
                {
                    var currentName = AddSubprocessPrefix(Subprocess, $"{element.Name} _");
                    var targetName = FutureElementName(boundaryTimerEvent);

                    var comment = TransitionCommentWithTimeout(2, currentName, targetName);
                    var matchLine = CurrentStateMatching(currentName, PlutusTimeoutRedeemer.Type.Name);
                    var guardLine = GuardLine(expectedVal: true);
                    var returningJust = ReturningJust(
                        new string[]
                        {
                            $"Constraints.mustValidateIn (from $ 1 + {PlutusCode.ProperlyBracketed(boundaryTimerEvent.TimerDefinition)})"
                        },
                        NonTxTransitionVisitor.TransitionFunctionSignature.Name +
                            " $ datum { contractState = " + targetName + " }",
                        "v");
                    var whereStatements = WhereStatements(element,
                        TxUserDefinedStatement.UserDefinedExpectedValue);

                    resultCode = resultCode
                        .Append(comment)
                        .Append(matchLine)
                        .Append(guardLine)
                        .Append(returningJust)
                        .Append(whereStatements)
                        .Append(PlutusLine.Empty);
                }
            }

            //Continue
            var target = element.Outgoing;
            return resultCode
                .Append(SingleOutputCommonTransition(element, target));
        }

        /// <inheritdoc/>
        public override IPlutusCode Visit(ContractScriptActivity element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            var target = element.Outgoing;
            return SingleOutputCommonTransition(element, target);
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
            return PlutusCode.Empty;
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
