using System.Reflection;
using System.Xml.Linq;
using DasContract.Abstraction;
using DasContract.Blockchain.Plutus;
using DasContract.Blockchain.Plutus.Console.Tests.DemoContracts;
using DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels;
using DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels.Properties;
using DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels.Properties.Dictionary;
using DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels.Properties.Enum;
using DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels.Properties.Primitive;
using DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels.Properties.Reference;
using DasContract.Blockchain.Plutus.Data.DasContractConversion.Processes;
using DasContract.Blockchain.Plutus.Data.DasContractConversion.Processes.Activities.MultiInstance;
using DasContract.Blockchain.Plutus.Data.Processes;
using DasContract.Blockchain.Plutus.Data.Processes.Process;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Activities;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Events;
using DasContract.Blockchain.Plutus.Data.Users;
using DasContract.Blockchain.Plutus.Generators;
using TextCopy;

//Preprogrammed contract
//var contract = TransitionDemoContract.Get();
//Console.WriteLine(new PlutusGenerator().GeneratePlutusContract(contract));

//Loaded DasContract
string path = @"../../../Lock funds.dascontract";
var lines = File.ReadAllLines(path);
var fileContent = string.Join(Environment.NewLine, lines);
var xElement = XElement.Parse(fileContent);
var contract = new Contract(xElement);

var plutusContract = PlutusContractConvertor.Default.Convert(contract);

//Remove assignees for debug
var userActivities = plutusContract.Processes.AllProcesses
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
    userActivity.Assignee = null;
    userActivity.CandidateUsers = new List<ContractUser>();
    userActivity.CandidateRoles = new List<ContractRole>();
}

//Generate
var contractCode = PlutusContractGenerator.Default(plutusContract).Generate();
Console.WriteLine(contractCode.InString());

new Clipboard().SetText(contractCode.InString());
Console.WriteLine("COPIED TO CLIPBOARD");


