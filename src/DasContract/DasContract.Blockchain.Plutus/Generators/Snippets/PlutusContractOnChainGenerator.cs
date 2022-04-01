using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;
using DasContract.Blockchain.Plutus.Code.Types.Determined;
using DasContract.Blockchain.Plutus.Code.Types.Premade;
using DasContract.Blockchain.Plutus.Code.Types.Temporary;
using DasContract.Blockchain.Plutus.Data;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Activities;
using DasContract.Blockchain.Plutus.Transitions.NonTx;
using DasContract.Blockchain.Plutus.Transitions.Tx;

namespace DasContract.Blockchain.Plutus.Generators.Snippets
{
    public class PlutusContractOnChainGenerator : ICodeGenerator
    {
        private readonly PlutusContract contract;

        public PlutusContractOnChainGenerator(PlutusContract contract)
        {
            this.contract = contract;
        }

        public IPlutusCode Generate()
        {
            var onChain = new PlutusSectionComment(0, "ON-CHAIN CODE")
                .Append(PlutusLine.Empty);

            //Lovelaces
            var lovelacesSig = new PlutusFunctionSignature(0, "lovelaces", new INamable[]
            {
                PlutusValue.Type,
                PlutusInteger.Type,
            });
            var lovelaces = new PlutusOnelineFunction(0, lovelacesSig, Array.Empty<string>(),
                "Ada.getLovelace . Ada.fromValue");

            onChain = onChain
                .Append(new PlutusPragma(0, $"INLINABLE {lovelacesSig.Name}"))
                .Append(lovelacesSig)
                .Append(lovelaces)
                .Append(PlutusLine.Empty);

            //Candidate users and roles constraint
            var candidateUsersAndRolesConstraintSig = new PlutusFunctionSignature(0, "candidateUsersAndRolesConstraint", new INamable[]
            {
                PlutusContractParam.Type,
                PlutusList.Type(PlutusBuiltinByteString.Type),
                PlutusList.Type(PlutusBuiltinByteString.Type),
                PlutusUnspecifiedDataType.Type("TxConstraints Void Void"),
            });
            var candidateUsersAndRolesConstraint = new PlutusFunction(0, candidateUsersAndRolesConstraintSig, new string[]
            {
                "param",
                "candidateUserNames",
                "candidateRoleNames",
            }, new IPlutusLine[]
            {

                new PlutusRawLine(1, "if PlutusTx.Prelude.null candidateUserNames && PlutusTx.Prelude.null candidateRoleNames then"),
                    new PlutusRawLine(2, "mempty"),
                new PlutusRawLine(1, "else"),
                    new PlutusRawLine(2, "Constraints.mustSatisfyAnyOf candidatesConstraints"),
                PlutusLine.Empty,

                    new PlutusRawLine(1, "where"),


                        new PlutusRawLine(2, "candidateRoles :: [Role]"),
                        new PlutusRawLine(2, "candidateRoles = PlutusTx.Prelude.map (roleByNameParam param) candidateRoleNames"),
                        PlutusLine.Empty,

                        new PlutusRawLine(2, "candidateUsers :: [User]"),
                        new PlutusRawLine(2, "candidateUsers = PlutusTx.Prelude.map (userByNameParam param) candidateUserNames"),
                        PlutusLine.Empty,

                        new PlutusRawLine(2, "isCandidateUser :: User -> Bool"),
                        new PlutusRawLine(2, "isCandidateUser u ="),
                            new PlutusRawLine(3, "u `elem` candidateUsers ||"),
                            new PlutusRawLine(3, "any (\\cr -> cr `elem` uRoles u) candidateRoles"),
                        PlutusLine.Empty,

                        new PlutusRawLine(2, "candidates :: [User]"),
                        new PlutusRawLine(2, "candidates = PlutusTx.Prelude.filter isCandidateUser (cpUsers param)"),
                        PlutusLine.Empty,

                        new PlutusRawLine(2, "candidatesConstraints :: [TxConstraints Void Void]"),
                        new PlutusRawLine(2, "candidatesConstraints = PlutusTx.Prelude.map (Constraints.mustBeSignedBy . uAddress) candidates"),
            });

            onChain = onChain
                .Append(new PlutusPragma(0, $"INLINABLE {candidateUsersAndRolesConstraintSig.Name}"))
                .Append(candidateUsersAndRolesConstraintSig)
                .Append(candidateUsersAndRolesConstraint)
                .Append(PlutusLine.Empty);

            // -- Form validations -------------------------------
            onChain = onChain
                   .Append(new PlutusSubsectionComment(0, "Form validations"));

            var formValidationSig = new PlutusFunctionSignature(0, "userDefinedFormValidation", new INamable[]
            {
                PlutusContractParam.Type,
                PlutusContractDatum.Type,
                PlutusContractRedeemer.Type,
                PlutusValue.Type,
                PlutusBool.Type,
            });
            onChain = onChain
                .Append(formValidationSig)
                .Append(PlutusLine.Empty);

            var userActivities = contract.Processes.AllProcesses
                .Aggregate(
                    new List<ContractUserActivity>(),
                    (acc, item) =>
                    {
                        return acc
                            .Concat(item.ProcessElements.OfType<ContractUserActivity>())
                            .ToList();
                    });

            foreach (var userActivity in userActivities)
            {
                var validationLines = userActivity.FormValidationCodeLines;
                if (validationLines.Count() == 0)
                    validationLines = validationLines.Append("True");

                var validationFunction = new PlutusFunction(0, formValidationSig, new string[]
                {
                    "param",
                    "datum",
                    $"({PlutusUserActivityRedeemer.Type(userActivity).Name} form)",
                    "val",
                },
                validationLines.Select(e => new PlutusRawLine(1, e)));

                onChain = onChain
                   .Append(validationFunction)
                   .Append(PlutusLine.Empty);
            }

            var defaultValidation = new PlutusOnelineFunction(0, formValidationSig, new string[]
            {
                "_",
                "_",
                "_",
                "_",
            }, "False");
            onChain = onChain
                .Append(defaultValidation)
                .Append(PlutusLine.Empty);

            // -- NonTx transitions ------------------------------
            onChain = onChain
                   .Append(new PlutusSubsectionComment(0, "NonTx transitions"));

            var scriptTransitionSig = NonTxTransitionVisitor.TransitionFunctionSignature;

            onChain = onChain
                .Append(scriptTransitionSig)
                .Append(PlutusLine.Empty);

            //Root process
            if (contract.Processes.Main.StartEvent is null)
                throw new Exception("The start event of the main process is null");
            var nonTxTransitionVisitor = new NonTxTransitionVisitor();
            var nonTxTransitionsRoot = nonTxTransitionVisitor.Visit(contract.Processes.Main.StartEvent);
            onChain = onChain
                .Append(new PlutusComment(0, "--> MAIN PROCESS"))
                .Append(nonTxTransitionsRoot);

            //Subprocesses
            foreach (var subprocess in contract.Processes.Subprocesses)
            {
                if (subprocess.StartEvent is null)
                    throw new Exception($"The start event of the {subprocess.Id} process is null");

                var nonTxTransitionSubprocessVisitor = new NonTxTransitionVisitor(subprocess);
                var nonTxTransitionsSubprocess = nonTxTransitionSubprocessVisitor.Visit(subprocess.StartEvent);
                onChain = onChain
                    .Append(new PlutusComment(0, $"--> {subprocess.Name.ToUpperInvariant()}"))
                    .Append(nonTxTransitionsSubprocess);
            }

            //Identity transition
            var identityTransition = new PlutusOnelineFunction(0, scriptTransitionSig, new string[] { "d" }, "d");
            onChain = onChain
                .Append(new PlutusComment(0, "--> DEFAULT"))
                .Append(identityTransition)
                .Append(PlutusLine.Empty);

            // -- Tx transition function -------------------------
            onChain = onChain
                  .Append(new PlutusSubsectionComment(0, "Tx transition function"));

            var txTransitionSig = TxTransitionVisitor.TransitionFunctionSignature;
            IPlutusCode txTransition = new PlutusFunction(0, txTransitionSig, new string[]
            {
                "cParam",
                "cState",
                "cRedeemer"
            }, new IPlutusLine[]
            {
                PlutusLine.Empty,
                new PlutusComment(1,    $"  (ContractParam, ContractDatum                       , Value            , ContractRedeemer )"),
                new PlutusRawLine(1, $"case (cParam       , {NonTxTransitionVisitor.TransitionFunctionSignature.Name} (stateData cState), stateValue cState, cRedeemer        ) of"),
                PlutusLine.Empty,
            });
            txTransition = txTransition.Prepend(txTransitionSig);

            //Root process
            var txTransitionVisitor = new TxTransitionVisitor();
            var txTransitionsRoot = txTransitionVisitor.Visit(contract.Processes.Main.StartEvent);
            txTransition = txTransition
                .Append(new PlutusComment(2, "--> MAIN PROCESS"))
                .Append(txTransitionsRoot);

            //Subprocesses
            foreach (var subprocess in contract.Processes.Subprocesses)
            {
                if (subprocess.StartEvent is null)
                    throw new Exception($"The start event of the {subprocess.Id} process is null");

                var txTransitionSubprocessVisitor = new TxTransitionVisitor(subprocess);
                var txTransitionsSubprocess = txTransitionSubprocessVisitor.Visit(subprocess.StartEvent);
                txTransition = txTransition
                    .Append(new PlutusComment(2, $"--> {subprocess.Name.ToUpperInvariant()}"))
                    .Append(txTransitionsSubprocess);
            }

            //Default case
            txTransition = txTransition
                .Append(new PlutusComment(2, "--> DEFAULT"))
                .Append(new PlutusRawLine(2, "_ -> Nothing"));

            onChain = onChain
                .Append(txTransition)
                .Append(PlutusLine.Empty);


            //Extra validator
            var extraValidatorSig = new PlutusFunctionSignature(0, "extraValidator", new INamable[]
            {
                PlutusContractParam.Type,
                PlutusContractDatum.Type,
                PlutusContractRedeemer.Type,
                PlutusScriptContext.Type,
                PlutusBool.Type
            });

            var extraValidatorCode = contract.GlobalValidationCodeLines
                    .Select(e => new PlutusRawLine(1, e));
            if (extraValidatorCode.Count() == 0)
                extraValidatorCode = extraValidatorCode.Append(new PlutusRawLine(1, "True"));

            var extraValidator = new PlutusFunction(0, extraValidatorSig, new string[]
            {
                "param",
                "datum",
                "redeemer",
                "context"
            }, extraValidatorCode);

            onChain = onChain
                .Append(new PlutusPragma(0, $"INLINABLE {extraValidatorSig.Name}"))
                .Append(extraValidatorSig)
                .Append(extraValidator)
                .Append(PlutusLine.Empty);

            //Contract finished
            var contractFinishedSig = new PlutusFunctionSignature(0, "contractFinished", new INamable[]
            {
                PlutusContractDatum.Type,
                PlutusBool.Type
            });
            var contractFinishedSuccess = new PlutusOnelineFunction(0, contractFinishedSig, new string[]
            {
                "ContractDatum { contractState = ContractFinished }",
            }, "True");
            var contractFinishedFallback = new PlutusOnelineFunction(0, contractFinishedSig, new string[]
            {
                "_",
            }, "False");

            onChain = onChain
                .Append(new PlutusPragma(0, $"INLINABLE {contractFinishedSig.Name}"))
                .Append(contractFinishedSig)
                .Append(contractFinishedSuccess)
                .Append(contractFinishedFallback)
                .Append(PlutusLine.Empty);

            //State machine
            var stateMachineSig = new PlutusFunctionSignature(0, "contractStateMachine", new INamable[]
            {
                PlutusContractParam.Type,
                PlutusStateMachine.Type(PlutusContractDatum.Type, PlutusContractRedeemer.Type)
            });
            var stateMachine = new PlutusFunction(0, stateMachineSig, new string[]
            {
                "param",
            }, new IPlutusLine[]
            {
                new PlutusRawLine(1, "StateMachine {"),
                    new PlutusRawLine(2, $"smTransition = {txTransitionSig.Name} param,"),
                    new PlutusRawLine(2, $"smFinal = {contractFinishedSig.Name},"),
                    new PlutusRawLine(2, $"smCheck = {extraValidatorSig.Name} param,"),
                    new PlutusRawLine(2, $"smThreadToken = Just $ cpToken param"),
                new PlutusRawLine(1, "}"),
            });

            onChain = onChain
                .Append(new PlutusPragma(0, $"INLINABLE {stateMachineSig.Name}"))
                .Append(stateMachineSig)
                .Append(stateMachine)
                .Append(PlutusLine.Empty);

            //Mk contract validator
            var mkContractValidatorSig = new PlutusFunctionSignature(0, "mkContractValidator", new INamable[]
            {
                PlutusContractParam.Type,
                PlutusContractDatum.Type,
                PlutusContractRedeemer.Type,
                PlutusScriptContext.Type,
                PlutusBool.Type
            });
            var mkContractValidator = new PlutusOnelineFunction(0, mkContractValidatorSig, new string[]
            {
                "param",
            }, "mkValidator $ contractStateMachine param");

            onChain = onChain
                .Append(new PlutusPragma(0, $"INLINABLE {mkContractValidatorSig.Name}"))
                .Append(mkContractValidatorSig)
                .Append(mkContractValidator)
                .Append(PlutusLine.Empty);

            //Contract type
            onChain = onChain
                .Append(new PlutusRawLine(0, $"type {PlutusContractType.Type.Name} = {PlutusStateMachine.Type(PlutusContractDatum.Type, PlutusContractRedeemer.Type).Name}"))
                .Append(PlutusLine.Empty);

            //Typed contract validator
            var typedContractValidatorSig = new PlutusFunctionSignature(0, "typedContractValidator", new INamable[]
            {
                PlutusContractParam.Type,
                PlutusUnspecifiedDataType.Type("Scripts.TypedValidator ContractType"),
            });
            var typedContractValidator = new PlutusFunction(0, typedContractValidatorSig, new string[]
            {
                "param",
            }, new IPlutusLine[]
            {
                new PlutusRawLine(1, $"Scripts.mkTypedValidator @{PlutusContractType.Type.Name}"),
                    new PlutusRawLine(2, $"($$(PlutusTx.compile [|| {mkContractValidatorSig.Name} ||]) `PlutusTx.applyCode` PlutusTx.liftCode param)"),
                    new PlutusRawLine(2, $"$$(PlutusTx.compile [|| wrap ||])"),
                new PlutusRawLine(1, "where"),
                    new PlutusRawLine(2, $"wrap = Scripts.wrapValidator @{PlutusContractDatum.Type.Name} @{PlutusContractRedeemer.Type.Name}"),
            });

            onChain = onChain
                .Append(typedContractValidatorSig)
                .Append(typedContractValidator)
                .Append(PlutusLine.Empty);

            //Contract client
            var contractClientSig = new PlutusFunctionSignature(0, "contractClient", new INamable[]
            {
                PlutusContractParam.Type,
                PlutusStateMachineClient.Type(PlutusContractDatum.Type, PlutusContractRedeemer.Type),
            });
            var contractClient = new PlutusOnelineFunction(0, contractClientSig, new string[]
            {
                "param",
            }, $"mkStateMachineClient $ StateMachineInstance (contractStateMachine param) ({typedContractValidatorSig.Name} param)");

            onChain = onChain
               .Append(contractClientSig)
               .Append(contractClient)
               .Append(PlutusLine.Empty);

            //Validator
            var validatorSig = new PlutusFunctionSignature(0, "validator", new INamable[]
            {
                PlutusContractParam.Type,
                PlutusValidator.Type
            });
            var validator = new PlutusOnelineFunction(0, validatorSig,
                Array.Empty<string>(),
                $"Scripts.validatorScript . {typedContractValidatorSig.Name}");

            onChain = onChain
               .Append(validatorSig)
               .Append(validator)
               .Append(PlutusLine.Empty);

            //valHash
            var valHashSig = new PlutusFunctionSignature(0, "valHash", new INamable[]
            {
                PlutusContractParam.Type,
                PlutusUnspecifiedDataType.Type("Ledger.ValidatorHash"),
            });
            var valHash = new PlutusOnelineFunction(0, valHashSig,
                Array.Empty<string>(),
                $"Scripts.validatorHash . {typedContractValidatorSig.Name}");

            onChain = onChain
               .Append(valHashSig)
               .Append(valHash)
               .Append(PlutusLine.Empty);

            //valHash
            var scrAddressSig = new PlutusFunctionSignature(0, "scrAddress", new INamable[]
            {
                PlutusContractParam.Type,
                PlutusUnspecifiedDataType.Type("Ledger.Address"),
            });
            var scrAddress = new PlutusOnelineFunction(0, scrAddressSig,
                Array.Empty<string>(),
                $"scriptAddress . {validatorSig.Name}");

            onChain = onChain
               .Append(scrAddressSig)
               .Append(scrAddress)
               .Append(PlutusLine.Empty);

            return onChain;
        }
    }
}
