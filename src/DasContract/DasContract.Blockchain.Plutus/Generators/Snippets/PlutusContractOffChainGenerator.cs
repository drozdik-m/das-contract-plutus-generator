using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;
using DasContract.Blockchain.Plutus.Code.Types.Determined;
using DasContract.Blockchain.Plutus.Code.Types.Premade;
using DasContract.Blockchain.Plutus.Code.Types.Temporary;
using DasContract.Blockchain.Plutus.Data;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using DasContract.Blockchain.Plutus.Transitions;
using DasContract.Blockchain.Plutus.Transitions.NonTx;

namespace DasContract.Blockchain.Plutus.Generators.Snippets
{
    public class PlutusContractOffChainGenerator : ICodeGenerator
    {
        private readonly PlutusContract contract;

        public PlutusContractOffChainGenerator(PlutusContract contract)
        {
            this.contract = contract;
        }

        public IPlutusCode Generate()
        {
            var offChain = new PlutusSectionComment(0, "OFF-CHAIN CODE")
                .Append(PlutusLine.Empty);

            //Map err
            var mapErrSig = new PlutusFunctionSignature(0, "mapErr", new INamable[]
            {
                PlutusContractMonad.Type(
                    PlutusUnspecifiedDataType.Type("w"),
                    PlutusUnspecifiedDataType.Type("s"),
                    PlutusUnspecifiedDataType.Type("SMContractError"),
                    PlutusUnspecifiedDataType.Type("a")
                    ),
                PlutusContractMonad.Type(
                    PlutusUnspecifiedDataType.Type("w"),
                    PlutusUnspecifiedDataType.Type("s"),
                    PlutusText.Type,
                    PlutusUnspecifiedDataType.Type("a")
                    ),
            });
            var mapErr = new PlutusOnelineFunction(0, mapErrSig,
                Array.Empty<string>(),
                $"mapError $ pack . show");

            offChain = offChain
               .Append(mapErrSig)
               .Append(mapErr)
               .Append(PlutusLine.Empty);

            //Create contract param
            var createContractParamSig = new PlutusFunctionSignature(0, "createContractParam", new INamable[]
            {
                PlutusThreadToken.Type,
                PlutusContractMonad.Type(
                    PlutusUnspecifiedDataType.Type("w"),
                    PlutusUnspecifiedDataType.Type("s"),
                    PlutusText.Type,
                    PlutusContractParam.Type
                    ),
            });
            var createContractParam = new PlutusFunction(0, createContractParamSig,
                new string[]
                {
                    "threadToken"
                },
                new IPlutusLine[]
                {
                    new PlutusRawLine(1, "return ContractParam {"),
                        new PlutusRawLine(2, $"cpUsers = users,"),
                        new PlutusRawLine(2, $"cpRoles = roles,"),
                        new PlutusRawLine(2, $"cpDefUser = def,"),
                        new PlutusRawLine(2, $"cpDefRole = def,"),
                        new PlutusRawLine(2, $"cpToken = threadToken"),
                    new PlutusRawLine(1, "}"),
                });

            offChain = offChain
               .Append(createContractParamSig)
               .Append(createContractParam)
               .Append(PlutusLine.Empty);

            //Init contract param
            var initContractParamSig = new PlutusFunctionSignature(0, "initContractParam", new INamable[]
            {
                PlutusContractMonad.Type(
                    PlutusUnspecifiedDataType.Type("w"),
                    PlutusUnspecifiedDataType.Type("s"),
                    PlutusText.Type,
                    PlutusContractParam.Type
                    ),
            });
            var initContractParam = new PlutusFunction(0, initContractParamSig,
                Array.Empty<string>(),
                new IPlutusLine[]
                {
                    new PlutusRawLine(1, "do"),
                        new PlutusRawLine(2, $"threadToken  <- {mapErrSig.Name} getThreadToken"),
                        new PlutusRawLine(2, $"{createContractParamSig.Name} threadToken"),
                });

            offChain = offChain
               .Append(initContractParamSig)
               .Append(initContractParam)
               .Append(PlutusLine.Empty);

            //On chain datum
            var onChainDatumSig = new PlutusFunctionSignature(0, "onChainDatum", new INamable[]
            {
                PlutusStateMachineClient.Type(PlutusContractDatum.Type, PlutusContractRedeemer.Type),
                PlutusContractMonad.Type(
                    PlutusUnspecifiedDataType.Type("w"),
                    PlutusUnspecifiedDataType.Type("s"),
                    PlutusText.Type,
                    PlutusContractDatum.Type
                    ),
            });
            var onChainDatum = new PlutusFunction(0, onChainDatumSig,
                new string[]
                {
                    "client"
                },
                new IPlutusLine[]
                {
                    new PlutusRawLine(1, "do"),
                        new PlutusRawLine(2, $"onChainState <- {mapErrSig.Name} $ getOnChainState client"),
                        new PlutusRawLine(2, $"case onChainState of"),
                            new PlutusRawLine(3, $"Nothing ->"),
                                new PlutusRawLine(4, $"throwError \"On-chain state could not be found\""),
                            new PlutusRawLine(3, $"Just (OnChainState o _ _, _) ->"),
                                new PlutusRawLine(4, $"return $ tyTxOutData o"),
                });

            offChain = offChain
               .Append(onChainDatumSig)
               .Append(onChainDatum)
               .Append(PlutusLine.Empty);

            //On chain value
            var onChainValueSig = new PlutusFunctionSignature(0, "onChainValue", new INamable[]
            {
                PlutusStateMachineClient.Type(PlutusContractDatum.Type, PlutusContractRedeemer.Type),
                PlutusContractMonad.Type(
                    PlutusUnspecifiedDataType.Type("w"),
                    PlutusUnspecifiedDataType.Type("s"),
                    PlutusText.Type,
                    PlutusValue.Type
                    ),
            });
            var onChainValue = new PlutusFunction(0, onChainValueSig,
                new string[]
                {
                    "client"
                },
                new IPlutusLine[]
                {
                    new PlutusRawLine(1, "do"),
                        new PlutusRawLine(2, $"onChainState <- {mapErrSig.Name} $ getOnChainState client"),
                        new PlutusRawLine(2, $"case onChainState of"),
                            new PlutusRawLine(3, $"Nothing ->"),
                                new PlutusRawLine(4, $"throwError \"On-chain state could not be found\""),
                            new PlutusRawLine(3, $"Just (OnChainState o _ _, _) ->"),
                                new PlutusRawLine(4, $"return $ txOutValue $ tyTxOutTxOut o"),
                });

            offChain = offChain
               .Append(onChainValueSig)
               .Append(onChainValue)
               .Append(PlutusLine.Empty);

            //Log on chain datum
            var logOnChainDatumSig = new PlutusFunctionSignature(0, "logOnChainDatum", new INamable[]
            {
                PlutusStateMachineClient.Type(PlutusContractDatum.Type, PlutusContractRedeemer.Type),
                PlutusContractMonad.Type(
                    PlutusUnspecifiedDataType.Type("w"),
                    PlutusUnspecifiedDataType.Type("s"),
                    PlutusText.Type,
                    PlutusVoid.Type
                    ),
            });
            var logOnChainDatum = new PlutusFunction(0, logOnChainDatumSig,
                new string[]
                {
                    "client"
                },
                new IPlutusLine[]
                {
                    new PlutusRawLine(1, "do"),
                        new PlutusRawLine(2, $"datum <- {onChainDatumSig.Name} client"),
                        new PlutusRawLine(2, $"logInfo @String $ \"--- \" ++ show datum"),
                });

            offChain = offChain
               .Append(logOnChainDatumSig)
               .Append(logOnChainDatum)
               .Append(PlutusLine.Empty);

            //Log on chain datum
            var validateInputFormSig = new PlutusFunctionSignature(0, "validateInputForm", new INamable[]
            {
                PlutusThreadToken.Type,
                PlutusStateMachineClient.Type(PlutusContractDatum.Type, PlutusContractRedeemer.Type),
                PlutusContractRedeemer.Type,
                PlutusContractMonad.Type(
                    PlutusUnspecifiedDataType.Type("w"),
                    PlutusUnspecifiedDataType.Type("s"),
                    PlutusText.Type,
                    PlutusVoid.Type
                    ),
            });
            var validateInputForm = new PlutusFunction(0, validateInputFormSig,
                new string[]
                {
                    "threadToken",
                    "client",
                    "redeemer"
                },
                new IPlutusLine[]
                {
                    new PlutusRawLine(1, "do"),
                        new PlutusRawLine(2, $"ocDatum <- {onChainDatumSig.Name} client"),
                        new PlutusRawLine(2, $"let datum = {NonTxTransitionVisitor.TransitionFunctionSignature.Name} ocDatum"),
                        new PlutusRawLine(2, $"val <- {onChainValueSig.Name} client"),
                        new PlutusRawLine(2, $"param <- {createContractParamSig.Name} threadToken"),
                        PlutusLine.Empty,
                        new PlutusRawLine(2, $"if userDefinedFormValidation param datum redeemer val then"),
                            new PlutusRawLine(3, $"logInfo @String $ \"--- form validated successfuly\""),
                        new PlutusRawLine(2, $"else"),
                            new PlutusRawLine(3, $"throwError \"User form failed validation\""),
                });

            offChain = offChain
               .Append(validateInputFormSig)
               .Append(validateInputForm)
               .Append(PlutusLine.Empty);

            // -- Endpoints --------------------------------------
            offChain = offChain
                  .Append(new PlutusSubsectionComment(0, "Endpoints"));

            //Generate endpoints
            var endpointsVisitor = new EndpointVisitor();
            var endpoints = endpointsVisitor.Visit(contract.Processes.Main.StartEvent);

            endpoints = endpoints.Append(endpointsVisitor.ContinueTimeoutActivityEndpoint());
            endpoints = endpoints.Append(endpointsVisitor.InitializeContractEndpoint());

            offChain = offChain
               .Append(endpoints)
               .Append(PlutusLine.Empty);

            // -- Contract schema --------------------------------
            offChain = offChain
                  .Append(new PlutusSubsectionComment(0, "Contract schema"));

            offChain = offChain
               .Append(endpointsVisitor.MakeSchema())
               .Append(PlutusLine.Empty);

            // -- Contract endpoints -----------------------------
            offChain = offChain
                  .Append(new PlutusSubsectionComment(0, "Contract endpoints"));

            offChain = offChain
               .Append(endpointsVisitor.MakeEndpoints())
               .Append(PlutusLine.Empty);

            return offChain;
        }
    }
}
