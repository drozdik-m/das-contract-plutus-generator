using DasContract.Blockchain.Plutus;
using DasContract.Blockchain.Plutus.Data;
using DasContract.Blockchain.Plutus.Data.Processes;
using DasContract.Blockchain.Plutus.Data.Processes.Process;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Activities;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Events;
using DasContract.Blockchain.Plutus.Data.Processes.Process.MultiInstances;

var mainEnd = new ContractEndEvent()
{
    Id = "ContractEnd",
    Name = "Dieee",
};
var helloWorldScript1 = new ContractScriptActivity()
{
    Id = "HelloWorldScript1"
};
var helloWorldUser1 = new ContractUserActivity()
{
    Id = "HelloWorldUser1"
};
var helloWorldScript2 = new ContractScriptActivity()
{
    Id = "HelloWorldScript2"
};
var mainStart = new ContractStartEvent()
{
    Id = "ContractStart",
    Name = "StartPlsMate"
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
    Name = "Dieee",
};
var subStart = new ContractStartEvent()
{
    Id = "ContractStartSub",
    Name = "StartPlsMate"
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
    }
};

var contract = new PlutusContract()
{
    Id = "contractId",
    Name = "NumberContract",
    Processes = processes,
};

Console.WriteLine(new PlutusGenerator().GeneratePlutusContract(contract));

