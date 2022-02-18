using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;
using DasContract.Blockchain.Plutus.Code.Convertors;
using DasContract.Blockchain.Plutus.Code.Convertors.DataType;
using DasContract.Blockchain.Plutus.Code.Types;
using DasContract.Blockchain.Plutus.Code.Types.Premade;
using DasContract.Blockchain.Plutus.Code.Types.Temporary;
using DasContract.Blockchain.Plutus.Data;
using DasContract.Blockchain.Plutus.Data.DataModels;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using DasContract.Blockchain.Plutus.Data.Processes.Process;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Activities;
using DasContract.Blockchain.Plutus.Utils;

namespace DasContract.Blockchain.Plutus
{
    public class PlutusGenerator
    {
        /// <summary>
        /// Generates Plutus code 
        /// </summary>
        /// <param name="contract">The contract data model to translate</param>
        /// <returns>Plutus code</returns>
        public IPlutusCode GeneratePlutusContract(PlutusContract contract)
        {
            //--- Pragma -----------------------------------------
            var pragmas = new PlutusCode(new List<IPlutusLine>()
            {
                new PlutusPragma(0, "LANGUAGE DataKinds"),
                new PlutusPragma(0, "LANGUAGE DeriveAnyClass"),
                new PlutusPragma(0, "LANGUAGE DeriveGeneric"),
                new PlutusPragma(0, "LANGUAGE FlexibleContexts"),
                new PlutusPragma(0, "LANGUAGE MultiParamTypeClasses"),
                new PlutusPragma(0, "LANGUAGE NoImplicitPrelude"),
                new PlutusPragma(0, "LANGUAGE OverloadedStrings"),
                new PlutusPragma(0, "LANGUAGE ScopedTypeVariables"),
                new PlutusPragma(0, "LANGUAGE TemplateHaskell"),
                new PlutusPragma(0, "LANGUAGE TypeApplications"),
                new PlutusPragma(0, "LANGUAGE TypeFamilies"),
                new PlutusPragma(0, "LANGUAGE TypeOperators"),
                PlutusLine.Empty,
                new PlutusPragma(0, "OPTIONS_GHC -fno-warn-unused-imports"),
                PlutusLine.Empty,
            });

            //--- Module -----------------------------------------
            var module = new PlutusCode(new List<IPlutusLine>()
            {
                new PlutusRawLine(0, "module PlutusContract"),
                    new PlutusRawLine(1, "(module PlutusContract)"),
                    new PlutusRawLine(1, "where"),

                PlutusLine.Empty,
                PlutusLine.Empty,
            });


            //--- Imports ----------------------------------------
            var imports = new PlutusCode(new List<IPlutusLine>()
            {
                new PlutusImport(0, "Control.Monad hiding (fmap)"),
                new PlutusImport(0, "Data.Aeson (ToJSON, FromJSON)"),
                new PlutusImport(0, "Data.Map as Map"),
                new PlutusImport(0, "Data.Default"),
                new PlutusImport(0, "Data.Text (Text, pack)"),
                new PlutusImport(0, "Data.Void (Void)"),
                new PlutusImport(0, "Data.Monoid (Last (..))"),
                new PlutusImport(0, "GHC.Generics (Generic)"),
                new PlutusImport(0, "Plutus.Contract"),
                new PlutusImport(0, "Plutus.Contract.StateMachine"),
                new PlutusImport(0, "Plutus.Contract.StateMachine.ThreadToken"),
                new PlutusImport(0, "PlutusTx (Data (..))"),
                new PlutusImport(0, "qualified PlutusTx"),
                new PlutusImport(0, "PlutusTx.Prelude hiding (Semigroup(..), unless)"),
                new PlutusImport(0, "Ledger hiding (singleton)"),
                new PlutusImport(0, "Ledger.Typed.Tx"),
                new PlutusImport(0, "Ledger.Constraints as Constraints"),
                new PlutusImport(0, "qualified Ledger.Typed.Scripts as Scripts"),
                new PlutusImport(0, "Ledger.Ada as Ada"),
                new PlutusImport(0, "Prelude (IO, Semigroup (..), Show (..), String)"),
                new PlutusImport(0, "Text.Printf (printf)"),
                new PlutusImport(0, "qualified PlutusTx.IsData as PlutusTx"),
                new PlutusImport(0, "Plutus.V1.Ledger.Value"),
                new PlutusImport(0, "Data.Char (GeneralCategory(CurrencySymbol))"),
                PlutusLine.Empty,
            });

            //----------------------------------------------------
            //--                 DATA MODELS                   ---
            //----------------------------------------------------
            IPlutusCode dataModels = new PlutusSectionComment(0, "DATA MODELS");

            // -- Sequential multi instance ----------------------
            dataModels = dataModels
                .Append(PlutusLine.Empty)
                .Append(new PlutusSubsectionComment(0, "Sequential multi instance"));
            var sequentialMultiInstanceData = new PlutusAlgebraicType("SequentialMultiInstance", new List<PlutusAlgebraicTypeConstructor>()
            {
                new PlutusAlgebraicTypeConstructor("ToLoop", new List<INamable>{ PlutusInteger.Type }),
                new PlutusAlgebraicTypeConstructor("LoopEnded", new List<INamable>())
            }, new List<string>()
            {
                "Show",
                "Generic",
                "FromJSON",
                "ToJSON"
            });
            dataModels = dataModels
                .Append(sequentialMultiInstanceData)
                .Append(new PlutusMakeLift(sequentialMultiInstanceData))
                .Append(new PlutusUnstableMakeIsData(sequentialMultiInstanceData))
                .Append(PlutusLine.Empty)
                .Append(new PlutusEq(sequentialMultiInstanceData))
                .Append(PlutusLine.Empty);

            var nextLoopFunctionSig = new PlutusFunctionSignature(0, "nextLoop", new List<INamable>
            {
                sequentialMultiInstanceData,
                sequentialMultiInstanceData,
            });
            dataModels = dataModels
                .Append(nextLoopFunctionSig)
                .Append(new PlutusOnelineFunction(0, nextLoopFunctionSig, new List<string>
                {
                    "LoopEnded"
                }, "LoopEnded"))
                .Append(new PlutusOnelineFunction(0, nextLoopFunctionSig, new List<string>
                {
                    "(ToLoop i)"
                }, "ToLoop $ i-1"))
                .Append(PlutusLine.Empty);

            var toSeqMultiInstanceSig = new PlutusFunctionSignature(0, "toSeqMultiInstance", new List<INamable>
            {
                PlutusInteger.Type,
                sequentialMultiInstanceData,
            });
            dataModels = dataModels
                .Append(toSeqMultiInstanceSig)
                .Append(new PlutusGuardFunction(0, toSeqMultiInstanceSig, new List<string>
                {
                    "i"
                }, new List<(string, string)>
                {
                    ("i <= 0", "LoopEnded"),
                    ("otherwise", "ToLoop i"),
                }))
                .Append(PlutusLine.Empty);

            var toNextSeqMultiInstanceSig = new PlutusFunctionSignature(0, "toNextSeqMultiInstance", new List<INamable>
            {
                PlutusInteger.Type,
                sequentialMultiInstanceData,
            });
            dataModels = dataModels
                .Append(toNextSeqMultiInstanceSig)
                .Append(new PlutusOnelineFunction(0, toNextSeqMultiInstanceSig, new List<string>
                {
                    "i"
                }, "toSeqMultiInstance $ i - 1"))
                .Append(PlutusLine.Empty);

            // -- Phase ------------------------------------------
            dataModels = dataModels
                    .Append(new PlutusSubsectionComment(0, "Phase"));

            //Subprocesses
            var subprocesses = contract.Processes.Subprocesses;
            foreach (var subprocess in subprocesses)
            {
                var subprocessPhases = new PlutusAlgebraicType(subprocess.Id,
                    subprocess.ProcessElements
                        .Select(e => ProcessElementToAlgTypeCtor(e, sequentialMultiInstanceData))
                , new List<string>()
                {
                    "Show",
                    "Generic",
                    "FromJSON",
                    "ToJSON"
                });

                dataModels = dataModels
                    .Append(subprocessPhases)
                    .Append(new PlutusMakeLift(subprocessPhases))
                    .Append(new PlutusUnstableMakeIsData(subprocessPhases))
                    .Append(PlutusLine.Empty)
                    .Append(new PlutusEq(subprocessPhases))
                    .Append(PlutusLine.Empty)
                    .Append(PlutusLine.Empty);
            }

            //Main process
            var contractState = new PlutusAlgebraicType("ContractState",
                    contract.Processes.Main.ProcessElements
                        .Select(e => ProcessElementToAlgTypeCtor(e, sequentialMultiInstanceData))
                        .Concat(contract.Processes.Subprocesses
                            .Select(e => new PlutusAlgebraicTypeConstructor(e.Id, new INamable[]
                            {
                                e
                            }))
                         )
                , new List<string>()
                {
                    "Show",
                    "Generic",
                    "FromJSON",
                    "ToJSON"
                });

            dataModels = dataModels
                .Append(contractState)
                .Append(new PlutusMakeLift(contractState))
                .Append(new PlutusUnstableMakeIsData(contractState))
                .Append(PlutusLine.Empty)
                .Append(new PlutusEq(contractState))
                .Append(PlutusLine.Empty)
                .Append(PlutusLine.Empty);

            // -- Datum ------------------------------------------
            dataModels = dataModels
                    .Append(new PlutusSubsectionComment(0, "Datum"));

            //Other models
            foreach(var entity in new EntitiesDependencyOrderingConvertor()
                .Convert(contract.DataModel.NonRootEntities))
            {
                var entityRecord = new PlutusRecord(entity.Id,
                    EntityToRecordMembers(entity)
                , new List<string>()
                {
                    "Show",
                    "Generic",
                    "FromJSON",
                    "ToJSON"
                });
                dataModels = dataModels
                    .Append(entityRecord)
                    .Append(new PlutusMakeLift(entity))
                    .Append(new PlutusUnstableMakeIsData(entity))
                    .Append(PlutusLine.Empty)
                    .Append(new PlutusEq(entityRecord))
                    .Append(PlutusLine.Empty)
                    .Append(new PlutusDefault(entityRecord))
                    .Append(PlutusLine.Empty);
            }

            //Root
            var contractDatum = new PlutusRecord("ContractDatum",
                    EntityToRecordMembers(contract.DataModel.RootEntity)
                        .Concat(new PlutusRecordMember[]
                        {
                            new PlutusRecordMember("contractState", contractState),
                            new PlutusRecordMember("stateStack", PlutusList.Type(contractState)),
                        })
                , new List<string>()
                {
                    "Show",
                    "Generic",
                    "FromJSON",
                    "ToJSON"
                });
            dataModels = dataModels
                .Append(contractDatum)
                .Append(new PlutusMakeLift(contractDatum))
                .Append(new PlutusUnstableMakeIsData(contractDatum))
                .Append(PlutusLine.Empty)
                .Append(new PlutusEq(contractDatum))
                .Append(PlutusLine.Empty)
                .Append(new PlutusDefault(contractDatum))
                .Append(PlutusLine.Empty)
                .Append(PlutusLine.Empty);

            //Initial datum
            var initialDatumSig = new PlutusFunctionSignature(0, "initialDatum", new INamable[]
            {
                contractDatum
            });
            dataModels = dataModels
                .Append(initialDatumSig)
                .Append(new PlutusOnelineFunction(0, initialDatumSig, Array.Empty<string>(), "def"))
                .Append(PlutusLine.Empty);

            //Push state
            var pushStateSig = new PlutusFunctionSignature(0, "pushState", new INamable[]
            {
                contractState,
                contractDatum,
                contractDatum
            });
            dataModels = dataModels
                .Append(pushStateSig)
                .Append(new PlutusFunction(0, pushStateSig, new string[]
                { 
                    "newState",
                    "datum"
                }, new IPlutusLine[]
                {
                    new PlutusRawLine(1, "datum {"),
                        new PlutusRawLine(2, "stateStack = newState : stateStack datum"),
                    new PlutusRawLine(1, "}"),
                }))
                .Append(PlutusLine.Empty);

            //Pop state
            var popStateSig = new PlutusFunctionSignature(0, "popState", new INamable[]
            {
                contractDatum,
                PlutusTuple.Type(contractState, contractDatum),
            });
            dataModels = dataModels
                .Append(popStateSig)
                .Append(new PlutusFunction(0, popStateSig, new string[]
                {
                    "datum",
                }, new IPlutusLine[]
                {
                    new PlutusRawLine(1, "(head $ stateStack datum, datum {"),
                        new PlutusRawLine(2, "stateStack = tail $ stateStack datum"),
                    new PlutusRawLine(1, "})"),
                }))
                .Append(PlutusLine.Empty);

            // -- User forms -------------------------------------
            dataModels = dataModels
                    .Append(new PlutusSubsectionComment(0, "User forms"));

            var userActivities = contract.Processes.Processes
                .Aggregate(
                    new List<ContractUserActivity>(),
                    (acc, item) =>
                    {
                        return acc
                            .Concat(item.ProcessElements.OfType<ContractUserActivity>())
                            .ToList();
                    });
            

            foreach(var userActivity in userActivities)
            {
                var form = userActivity.Form;
                var formName = userActivity.FormName;

                var convertor = new PrimitivePropertyToTypeConvertor(
                    new PrimitivePropertyTypeToPlutusConvertor());

                var formRecord = new PlutusRecord(formName,
                    form.Fields.Select(e => new PlutusRecordMember(
                        e.Name, convertor.Convert(e)))
                , new List<string>()
                {
                    "Show",
                    "Generic",
                    "FromJSON",
                    "ToJSON"
                });
                dataModels = dataModels
                    .Append(formRecord)
                    .Append(new PlutusMakeLift(formRecord))
                    .Append(new PlutusUnstableMakeIsData(formRecord))
                    .Append(PlutusLine.Empty)
                    .Append(new PlutusEq(formRecord))
                    .Append(PlutusLine.Empty);
            }

            // -- Redeemers --------------------------------------
            dataModels = dataModels
                   .Append(new PlutusSubsectionComment(0, "Redeemers"));

            var contractRedeemer = new PlutusAlgebraicType("ContractRedeemer",
                userActivities.Select(e => new PlutusAlgebraicTypeConstructor(e.Name + "Redeemer", new INamable[]
                    {
                        PlutusFutureDataType.Type(e.FormName)
                    }))
                .Append(new PlutusAlgebraicTypeConstructor("ContractFinishedRedeemer", Array.Empty<INamable>()))
                .Append(new PlutusAlgebraicTypeConstructor("TimeoutRedeemer", Array.Empty<INamable>())),
            new List<string>()
            {
                "Show",
                "Generic",
                "FromJSON",
                "ToJSON"
            });
            dataModels = dataModels
                .Append(contractRedeemer)
                .Append(new PlutusMakeLift(contractRedeemer))
                .Append(new PlutusUnstableMakeIsData(contractRedeemer))
                .Append(PlutusLine.Empty)
                .Append(new PlutusEq(contractRedeemer))
                .Append(PlutusLine.Empty);


            //Result
            return pragmas
                .Append(module)
                .Append(imports)
                .Append(dataModels);

        }

        /// <summary>
        /// Converts a process element into plutus algebraic type constructor
        /// </summary>
        /// <param name="element"></param>
        /// <param name="sequentialInstanceType"></param>
        /// <returns></returns>
        private PlutusAlgebraicTypeConstructor ProcessElementToAlgTypeCtor(
            ContractProcessElement element, 
            PlutusAlgebraicType sequentialInstanceType)
        {
            var types = new List<INamable>();
            if (element is ContractActivity)
            {
                var contractActivity = element as ContractActivity;
                if (!(contractActivity?.MultiInstance is null))
                    types.Add(sequentialInstanceType);
            }

            return new PlutusAlgebraicTypeConstructor(element.Id, types);
        }

        private IEnumerable<PlutusRecordMember> EntityToRecordMembers(ContractEntity entity)
        {
            var result = new List<PlutusRecordMember>();    

            //Primitive properties
            foreach(var property in entity.PrimitiveProperties)
            {
                result.Add(new PlutusRecordMember(
                    property.Name,
                    new PrimitivePropertyToTypeConvertor(
                            new PrimitivePropertyTypeToPlutusConvertor()
                        ).Convert(property)));
            }

            //Reference properties
            foreach (var property in entity.ReferenceProperties)
            {
                result.Add(new PlutusRecordMember(
                    property.Name,
                    new ReferencePropertyToTypeConvertor().Convert(property)));
            }

            //Dictionary properties
            foreach (var property in entity.DictionaryProperties)
            {
                var primitiveConv = new PrimitivePropertyTypeToPlutusConvertor();

                result.Add(new PlutusRecordMember(
                    property.Name,
                    new DictionaryPropertyToTypeConvertor(
                            primitiveConv,
                            new PrimitivePropertyToTypeConvertor(primitiveConv),
                            new ReferencePropertyToTypeConvertor()
                        ).Convert(property)));
            }

            return result;
        }
    }
}
