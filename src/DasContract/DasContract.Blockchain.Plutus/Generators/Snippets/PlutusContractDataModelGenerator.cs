using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;
using DasContract.Blockchain.Plutus.Code.Convertors.DataType;
using DasContract.Blockchain.Plutus.Code.Instances;
using DasContract.Blockchain.Plutus.Code.Keywords;
using DasContract.Blockchain.Plutus.Code.Types;
using DasContract.Blockchain.Plutus.Code.Types.Determined;
using DasContract.Blockchain.Plutus.Code.Types.Premade;
using DasContract.Blockchain.Plutus.Data;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using DasContract.Blockchain.Plutus.Data.Processes.Process;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Activities;
using DasContract.Blockchain.Plutus.Data.Processes.Process.MultiInstances;

namespace DasContract.Blockchain.Plutus.Generators.Snippets
{
    public class PlutusContractDataModelGenerator : ICodeGenerator
    {
        private readonly PlutusContract contract;

        public PlutusContractDataModelGenerator(PlutusContract contract)
        {
            this.contract = contract;
        }

        public IPlutusCode Generate()
        {
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
                        .Append(new PlutusAlgebraicTypeConstructor(
                            PlutusContractFinished.Type.Name,
                            Array.Empty<INamable>()))
                , new List<string>()
                {
                    "Show",
                    "Generic",
                    "FromJSON",
                    "ToJSON"
                });

            if (contract?.Processes?.Main?.StartEvent is null)
                throw new Exception("Main process or its start event is null");

            dataModels = dataModels
                .Append(contractState)
                .Append(new PlutusMakeLift(contractState))
                .Append(new PlutusUnstableMakeIsData(contractState))
                .Append(PlutusLine.Empty)
                .Append(new PlutusEq(contractState))
                .Append(PlutusLine.Empty)
                .Append(new PlutusDefault(contractState, new PlutusAlgebraicTypeConstructor(
                    contract.Processes.Main.StartEvent.Name,
                    Array.Empty<INamable>())))
                .Append(PlutusLine.Empty)
                .Append(PlutusLine.Empty);

            // -- Datum ------------------------------------------
            dataModels = dataModels
                    .Append(new PlutusSubsectionComment(0, "Datum"));

            //Enums
            foreach (var enumModel in contract.DataModel.Enums)
            {

                var enumCtors = enumModel.Values
                    .Select(e => new PlutusAlgebraicTypeConstructor(e, Array.Empty<INamable>()));
                var enumType = new PlutusAlgebraicType(enumModel.Name, enumCtors,
                    new List<string>()
                    {
                        "Show",
                        "Generic",
                        "FromJSON",
                        "ToJSON"
                    });
                dataModels = dataModels
                    .Append(enumType)
                    .Append(new PlutusMakeLift(enumModel))
                    .Append(new PlutusUnstableMakeIsData(enumModel))
                    .Append(PlutusLine.Empty)
                    .Append(new PlutusEq(enumType))
                    .Append(PlutusLine.Empty)
                    .Append(new PlutusDefault(enumType, enumCtors.First()))
                    .Append(PlutusLine.Empty);
            }

            //Other models
            foreach (var entity in new EntitiesDependencyOrderingConvertor()
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
            var contractDatum = new PlutusRecord(PlutusContractDatum.Type.Name,
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
                var form = userActivity.Form;

                var convertor = new PrimitivePropertyToTypeConvertor(
                    new PrimitivePropertyTypeToPlutusConvertor());

                var formRecord = new PlutusRecord(PlutusUserActivityForm.Type(userActivity).Name,
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

            var contractRedeemer = new PlutusAlgebraicType(PlutusContractRedeemer.Type.Name,
                userActivities.Select(e => new PlutusAlgebraicTypeConstructor(PlutusUserActivityRedeemer.Type(e).Name, new INamable[]
                    {
                        PlutusUserActivityForm.Type(e)
                    }))
                .Append(new PlutusAlgebraicTypeConstructor(PlutusContractFinishedRedeemer.Type.Name, Array.Empty<INamable>()))
                .Append(new PlutusAlgebraicTypeConstructor(PlutusTimeoutRedeemer.Type.Name, Array.Empty<INamable>())),
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

            // -- Users ------------------------------------------
            dataModels = dataModels
                   .Append(new PlutusSubsectionComment(0, "Users"));

            //Role
            var role = new PlutusRecord("Role", new PlutusRecordMember[]
            {
                new PlutusRecordMember("rName", PlutusBuiltinByteString.Type),
                new PlutusRecordMember("rDescription", PlutusBuiltinByteString.Type),
            }, new string[]
            {
                "Show",
                "Generic",
                "FromJSON",
                "ToJSON"
            });
            dataModels = dataModels
                .Append(role)
                .Append(new PlutusMakeLift(role))
                .Append(new PlutusUnstableMakeIsData(role))
                .Append(PlutusLine.Empty)
                .Append(new PlutusEq(role))
                .Append(PlutusLine.Empty)
                .Append(new PlutusDefault(role))
                .Append(PlutusLine.Empty);

            //User
            var user = new PlutusRecord("User", new PlutusRecordMember[]
            {
                new PlutusRecordMember("uName", PlutusBuiltinByteString.Type),
                new PlutusRecordMember("uAddress", PlutusPubKeyHash.Type),
                new PlutusRecordMember("uDescription", PlutusBuiltinByteString.Type),
                new PlutusRecordMember("uRoles", PlutusList.Type(role)),
            }, new string[]
            {
                "Show",
                "Generic",
                "FromJSON",
                "ToJSON"
            });
            dataModels = dataModels
                .Append(user)
                .Append(new PlutusMakeLift(user))
                .Append(new PlutusUnstableMakeIsData(user))
                .Append(PlutusLine.Empty)
                .Append(new PlutusEq(user))
                .Append(PlutusLine.Empty)
                .Append(new PlutusDefault(user))
                .Append(PlutusLine.Empty);

            //Roles
            var rolesSig = new PlutusFunctionSignature(0, "roles", new INamable[]
            {
                PlutusList.Type(role)
            });
            var roles = new PlutusFunction(0, rolesSig, Array.Empty<string>(), new IPlutusLine[]
            {
                new PlutusRawLine(1, "[")
            }.Concat(
                contract.Identities.Roles.Select((e, i) =>
                        new PlutusRawLine(2, "Role { " + $"rName = \"{e.Name}\", rDescription = \"{e.Description}\"" + "}" +
                        (i == contract.Identities.Roles.Count() - 1 ? "" : ",")))
                )
             .Append(new PlutusRawLine(1, "]")));

            dataModels = dataModels
                .Append(new PlutusPragma(0, $"INLINABLE {rolesSig.Name}"))
                .Append(rolesSig)
                .Append(roles)
                .Append(PlutusLine.Empty);


            //Role by name
            var roleByNameSig = new PlutusFunctionSignature(0, "roleByName", new INamable[]
            {
                PlutusBuiltinByteString.Type,
                role
            });
            var roleByName = new PlutusFunction(0, roleByNameSig, new string[]
            {
                "roleName"
            }, Array.Empty<IPlutusLine>())
                .Append(new PlutusLetIn(1, new IPlutusLine[]
            {
                new PlutusRawLine(2, "searchResult = find (\\x -> rName x == roleName) roles")
            }, new IPlutusLine[]
            {
                new PlutusRawLine(2, "case searchResult of"),
                    new PlutusRawLine(3, "Just a -> a"),
                    new PlutusRawLine(3, "Nothing -> def"),
            }));
            dataModels = dataModels
                .Append(new PlutusPragma(0, $"INLINABLE {roleByNameSig.Name}"))
                .Append(roleByNameSig)
                .Append(roleByName)
                .Append(PlutusLine.Empty);

            //Role by name param
            var roleByNameParamSig = new PlutusFunctionSignature(0, "roleByNameParam", new INamable[]
            {
                PlutusContractParam.Type,
                PlutusBuiltinByteString.Type,
                role
            });
            var roleByNameParam = new PlutusFunction(0, roleByNameParamSig, new string[]
            {
                "param",
                "roleName"
            }, Array.Empty<IPlutusLine>())
                .Append(new PlutusLetIn(1, new IPlutusLine[]
            {
                new PlutusRawLine(2, "searchResult = find (\\x -> rName x == roleName) (cpRoles param)")
            }, new IPlutusLine[]
            {
                new PlutusRawLine(2, "case searchResult of"),
                    new PlutusRawLine(3, "Just a -> a"),
                    new PlutusRawLine(3, "Nothing -> cpDefRole param"),
            }));
            dataModels = dataModels
                .Append(new PlutusPragma(0, $"INLINABLE {roleByNameParamSig.Name}"))
                .Append(roleByNameParamSig)
                .Append(roleByNameParam)
                .Append(PlutusLine.Empty);

            //Pub key string as pub key hash
            var pubKeyStringAsPubKeyHashSig = new PlutusFunctionSignature(0, "pubKeyStringAsPubKeyHash", new INamable[]
            {
                PlutusString.Type,
                PlutusPubKeyHash.Type
            });
            var pubKeyStringAsPubKeyHash = new PlutusFunction(0, pubKeyStringAsPubKeyHashSig, new string[]
            {
                "key",
            }, new IPlutusLine[]
            {
                new PlutusRawLine(1, "case fromHex (fromString key) of"),
                    new PlutusRawLine(2, "Right b -> pubKeyHash $ PubKey {"),
                        new PlutusRawLine(3, "getPubKey = b"),
                    new PlutusRawLine(2, "}"),
                    new PlutusRawLine(2, "Left _ -> uAddress (def :: User)"),
            });

            dataModels = dataModels
                .Append(new PlutusPragma(0, $"INLINABLE {pubKeyStringAsPubKeyHashSig.Name}"))
                .Append(pubKeyStringAsPubKeyHashSig)
                .Append(pubKeyStringAsPubKeyHash)
                .Append(PlutusLine.Empty);


            //Users
            var usersSig = new PlutusFunctionSignature(0, "users", new INamable[]
            {
                PlutusList.Type(user)
            });
            var users = new PlutusFunction(0, usersSig, Array.Empty<string>(), new IPlutusLine[]
            {
                new PlutusRawLine(1, "[")
            }.Concat(
                contract.Identities.Users.Select((e, i) =>
                        new PlutusRawLine(2, "User { " + $"uName = \"{e.Name}\", uDescription = \"{e.Description}\", uAddress = {pubKeyStringAsPubKeyHashSig.Name} \"{e.Address}\", " +
                        $"uRoles = [{string.Join(", ", e.Roles.Select(e => $"{roleByNameSig.Name} \"{e.Name}\""))}]" + "}" +
                        (i == contract.Identities.Users.Count() - 1 ? "" : ",")))
                )
             .Append(new PlutusRawLine(1, "]")));

            dataModels = dataModels
                .Append(new PlutusPragma(0, $"INLINABLE {usersSig.Name}"))
                .Append(usersSig)
                .Append(users)
                .Append(PlutusLine.Empty);

            //User by name
            var userByNameSig = new PlutusFunctionSignature(0, "userByName", new INamable[]
            {
                PlutusBuiltinByteString.Type,
                user
            });
            var userByName = new PlutusFunction(0, userByNameSig, new string[]
            {
                "userName"
            }, Array.Empty<IPlutusLine>())
                .Append(new PlutusLetIn(1, new IPlutusLine[]
            {
                new PlutusRawLine(2, "searchResult = find (\\x -> uName x == userName) users")
            }, new IPlutusLine[]
            {
                new PlutusRawLine(2, "case searchResult of"),
                    new PlutusRawLine(3, "Just a -> a"),
                    new PlutusRawLine(3, "Nothing -> def"),
            }));
            dataModels = dataModels
                .Append(new PlutusPragma(0, $"INLINABLE {userByNameSig.Name}"))
                .Append(userByNameSig)
                .Append(userByName)
                .Append(PlutusLine.Empty);

            //User by name param
            var userByNameParamSig = new PlutusFunctionSignature(0, "userByNameParam", new INamable[]
            {
                PlutusContractParam.Type,
                PlutusBuiltinByteString.Type,
                user
            });
            var userByNameParam = new PlutusFunction(0, userByNameParamSig, new string[]
            {
                "param",
                "userName"
            }, Array.Empty<IPlutusLine>())
                .Append(new PlutusLetIn(1, new IPlutusLine[]
            {
                new PlutusRawLine(2, "searchResult = find (\\x -> uName x == userName) (cpUsers param)")
            }, new IPlutusLine[]
            {
                new PlutusRawLine(2, "case searchResult of"),
                    new PlutusRawLine(3, "Just a -> a"),
                    new PlutusRawLine(3, "Nothing -> cpDefUser param"),
            }));
            dataModels = dataModels
                .Append(new PlutusPragma(0, $"INLINABLE {userByNameParamSig.Name}"))
                .Append(userByNameParamSig)
                .Append(userByNameParam)
                .Append(PlutusLine.Empty);


            // -- Contract param ---------------------------------
            dataModels = dataModels
                   .Append(new PlutusSubsectionComment(0, "Contract param"));

            var contractParam = new PlutusRecord(PlutusContractParam.Type.Name, new PlutusRecordMember[]
            {
                new PlutusRecordMember("cpUsers", PlutusList.Type(user)),
                new PlutusRecordMember("cpRoles", PlutusList.Type(role)),
                new PlutusRecordMember("cpDefUser", user),
                new PlutusRecordMember("cpDefRole", role),
                new PlutusRecordMember("cpToken", PlutusThreadToken.Type),

            }, new string[]
            {
                "Show",
                "Generic",
                "FromJSON",
                "ToJSON"
            });

            dataModels = dataModels
                .Append(contractParam)
                .Append(new PlutusMakeLift(contractParam))
                .Append(PlutusLine.Empty);

            return dataModels;
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
                if (contractActivity?.MultiInstance is ContractSequentialMultiInstance)
                    types.Add(sequentialInstanceType);
            }

            return new PlutusAlgebraicTypeConstructor(element.Id, types);
        }

        /// <summary>
        /// Translates an entity into plutus record members
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private IEnumerable<PlutusRecordMember> EntityToRecordMembers(ContractEntity entity)
        {
            var result = new List<PlutusRecordMember>();

            //Primitive properties
            foreach (var property in entity.PrimitiveProperties)
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
                            new ReferencePropertyToTypeConvertor(),
                            new EnumPropertyToTypeConvertor()
                        ).Convert(property)));
            }

            //Enum properties
            foreach (var property in entity.EnumProperties)
            {
                result.Add(new PlutusRecordMember(
                    property.Name,
                    new EnumPropertyToTypeConvertor().Convert(property)));
            }

            return result;
        }
    }
}
