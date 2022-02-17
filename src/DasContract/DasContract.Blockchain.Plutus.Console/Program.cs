using DasContract.Blockchain.Plutus;
using DasContract.Blockchain.Plutus.Data;
using DasContract.Blockchain.Plutus.Data.DataModels;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Dictionary;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;
using DasContract.Blockchain.Plutus.Data.Forms;
using DasContract.Blockchain.Plutus.Data.Processes;
using DasContract.Blockchain.Plutus.Data.Processes.Process;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Activities;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Events;
using DasContract.Blockchain.Plutus.Data.Processes.Process.MultiInstances;
using DasContract.Editor.Entities.DataModels.Entities.Properties.Reference;

var mainEnd = new ContractEndEvent()
{
    Id = "ContractEnd",
};
var helloWorldScript1 = new ContractScriptActivity()
{
    Id = "HelloWorldScript1"
};
var helloWorldUser1 = new ContractUserActivity()
{
    Id = "HelloWorldUser1",
    Form = new ContractForm()
    {
        Fields = new PrimitiveContractProperty[]
        {
            new PrimitiveContractProperty()
            {
                Id = "someProperty123",
                Cardinality = ContractPropertyCardinality.Single,
                IsMandatory = true,
                Type = PrimitiveContractPropertyType.Integer
            },
        }
    }
};
var helloWorldScript2 = new ContractScriptActivity()
{
    Id = "HelloWorldScript2"
};
var mainStart = new ContractStartEvent()
{
    Id = "ContractStart",
};
var mainProcess = new ContractProcess()
{
    IsMain = true,
    StartEvent = mainStart,
    Id = "MainProcess"
};
mainStart.Outgoing = helloWorldScript1;
helloWorldScript1.Outgoing = helloWorldUser1;
helloWorldUser1.Outgoing = helloWorldScript2;
helloWorldScript2.Outgoing = mainEnd;


var subEnd = new ContractEndEvent()
{
    Id = "ContractEndSub",
};
var subStart = new ContractStartEvent()
{
    Id = "ContractStartSub",
};
var subScript1 = new ContractScriptActivity()
{
    Id = "HelloWorldScript1Sub",
    MultiInstance = new ContractSequentialMultiInstance()
    {
        LoopCardinalityExpr = "3"
    }
};
var subUser1 = new ContractUserActivity()
{
    Id = "HelloWorldUser1Sub",
    BoundaryEvents = new ContractBoundaryEvent[]
    {
        new ContractTimerBoundaryEvent()
        {
            Id = "HelloWorldUser1SubTimeout",
            TimeOutDirection = subEnd
        },
    }

};

var subProcess = new ContractProcess()
{
    IsMain = false,
    StartEvent = subStart,
    Id = "Subprocess1"
};
subStart.Outgoing = subScript1;
subScript1.Outgoing = subUser1;
subUser1.Outgoing = subEnd;

var processes = new ContractProcesses()
{
    Processes = new List<ContractProcess>()
    {
        mainProcess,
        subProcess,
    },
};

var contractLogEntity = new ContractEntity()
{
    IsRoot = false,
    Id = "LogEntityId",
    PrimitiveProperties = new PrimitiveContractProperty[]
                {
                    new PrimitiveContractProperty()
                    {
                        Id = "logMessageId",
                        Cardinality = ContractPropertyCardinality.Single,
                        IsMandatory = true,
                        Type = PrimitiveContractPropertyType.Integer
                    },
                }
};

var contract = new PlutusContract()
{
    Id = "contractId",
    Name = "NumberContract",
    Processes = processes,
    DataModel = new ContractDataModel()
    {
        Entities = new ContractEntity[]
        {
            contractLogEntity,
            new ContractEntity()
            {
                IsRoot = true,
                Id = "RootEntityId",
                PrimitiveProperties = new PrimitiveContractProperty[]
                {
                    new PrimitiveContractProperty()
                    {
                        Id = "interestingNumberId",
                        Cardinality = ContractPropertyCardinality.Single,
                        IsMandatory = true,
                        Type = PrimitiveContractPropertyType.Integer
                    },
                    new PrimitiveContractProperty()
                    {
                        Id = "setNumberTimeoutId",
                        Cardinality = ContractPropertyCardinality.Single,
                        IsMandatory = true,
                        Type = PrimitiveContractPropertyType.Integer
                    },
                    new PrimitiveContractProperty()
                    {
                        Id = "optinalWalletId",
                        Cardinality = ContractPropertyCardinality.Single,
                        IsMandatory = false,
                        Type = PrimitiveContractPropertyType.Address
                    },
                    new PrimitiveContractProperty()
                    {
                        Id = "optinalBooleansId",
                        Cardinality = ContractPropertyCardinality.Collection,
                        IsMandatory = false,
                        Type = PrimitiveContractPropertyType.Bool
                    }
                },
                ReferenceProperties = new ReferenceContractProperty[]
                {
                    new ReferenceContractProperty()
                    {
                        Id = "logId",
                        Cardinality = ContractPropertyCardinality.Single,
                        IsMandatory = true,
                        Entity = contractLogEntity,
                        EntityId = contractLogEntity.Id
                    },
                    new ReferenceContractProperty()
                    {
                        Id = "optionalLogId",
                        Cardinality = ContractPropertyCardinality.Single,
                        IsMandatory = false,
                        Entity = contractLogEntity,
                        EntityId = contractLogEntity.Id
                    },

                    new ReferenceContractProperty()
                    {
                        Id = "collectionLogId",
                        Cardinality = ContractPropertyCardinality.Collection,
                        IsMandatory = true,
                        Entity = contractLogEntity,
                        EntityId = contractLogEntity.Id
                    },
                    new ReferenceContractProperty()
                    {
                        Id = "optionalCollectionLogId",
                        Cardinality = ContractPropertyCardinality.Collection,
                        IsMandatory = false,
                        Entity = contractLogEntity,
                        EntityId = contractLogEntity.Id
                    },

                },
                DictionaryProperties = new DictionaryContractProperty[]
                {
                    new DictionaryContractProperty()
                    {
                        Id = "simpleDictionaryId",
                        Cardinality = ContractPropertyCardinality.Single,
                        IsMandatory = true,
                        KeyType = PrimitiveContractPropertyType.Integer,
                        ValueType = new PrimitiveContractProperty()
                        {
                            Id = "DICT_ID",
                            Cardinality = ContractPropertyCardinality.Collection,
                            IsMandatory = true,
                            Type = PrimitiveContractPropertyType.Address
                        },
                    },
                    new DictionaryContractProperty()
                    {
                        Id = "referenceDictionaryId",
                        Cardinality = ContractPropertyCardinality.Single,
                        IsMandatory = true,
                        KeyType = PrimitiveContractPropertyType.Integer,
                        ValueType = new ReferenceContractProperty()
                        {
                            Id = "DICT_ID",
                            Cardinality = ContractPropertyCardinality.Single,
                            IsMandatory = true,
                            Entity = contractLogEntity,
                            EntityId= contractLogEntity.Id  
                        },
                    },
                    new DictionaryContractProperty()
                    {
                        Id = "crazyDictionaryId",
                        Cardinality = ContractPropertyCardinality.Single,
                        IsMandatory = true,
                        KeyType = PrimitiveContractPropertyType.Integer,
                        ValueType = new DictionaryContractProperty()
                        {
                            Cardinality = ContractPropertyCardinality.Collection,
                            IsMandatory = true,
                            KeyType = PrimitiveContractPropertyType.Bool,
                            ValueType = new ReferenceContractProperty()
                            {
                                Cardinality = ContractPropertyCardinality.Single,
                                IsMandatory = true,
                                Entity = contractLogEntity,
                                EntityId= contractLogEntity.Id
                            },
                        },
                    }
                }

            }
        }
    }
};

Console.WriteLine(new PlutusGenerator().GeneratePlutusContract(contract));

