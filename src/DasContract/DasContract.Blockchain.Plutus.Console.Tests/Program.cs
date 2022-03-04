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
using TextCopy;

//Preprogrammed contract
//var contract = TransitionDemoContract.Get();
//Console.WriteLine(new PlutusGenerator().GeneratePlutusContract(contract));

//Loaded DasContract
string path = @"../../../contract.dascontract";
var lines = File.ReadAllLines(path);
var fileContent = string.Join(Environment.NewLine, lines);
var xElement = XElement.Parse(fileContent);
var contract = new Contract(xElement);


//Data type convertors
var propertyCardinalityConvertor = new PropertyCardinalityConvertor();
var propertyTypeConvertor = new PrimitivePropertyTypeConvertor();
var primitivePropertyConvertor = new PrimitivePropertyConvertor(
    propertyTypeConvertor,
    propertyCardinalityConvertor);
var referencePropertyConvertor = new ReferencePropertyConvertor(
    propertyCardinalityConvertor);
var dictionaryPropertyConvertor = new DictionaryPropertyConvertor(
    propertyTypeConvertor);
var enumPropertyConvertor = new EnumPropertyConvertor(
    propertyCardinalityConvertor);
var propertyConvertor = new ContractPropertyConvertor(
    primitivePropertyConvertor,
    referencePropertyConvertor,
    dictionaryPropertyConvertor,
    enumPropertyConvertor);
var entityConvertor = new ContractEntityConvertor(
    propertyConvertor);
var contractEnumConvertor = new EnumConvertor();
var plutusDataModelConvertor = new ContractDataModelConvertor(
    entityConvertor,
    contractEnumConvertor);

//User convertor
var roleConvertor = new ContractRoleConvertor();
var userConvertor = new ContractUserConvertor();
var usersConvertor = new ContractUsersConvertor(
    roleConvertor,
    userConvertor);

//Process convertor
var multiInstanceConvertor = new ContractSequentialMultiInstanceConvertor();
var scriptActivityConvertor = new ContractScriptActivityConvertor(
    multiInstanceConvertor);
var contractFieldTypeConvertor = new ContractFieldTypeConvertor();
var userFormConvertor = new ContractFormConvertor(
    contractFieldTypeConvertor);
var userActivityConvertor = new ContractUserActivityConvertor(
    multiInstanceConvertor,
    userFormConvertor);
var timerBoundaryConvertor = new ContractTimerBoundaryEventConvertor();
var processConvertor = new ContractProcessConvertor(
    scriptActivityConvertor,
    userActivityConvertor,
    timerBoundaryConvertor);
var processesConvertor = new ContractProcessesConvertor(
    processConvertor);

//Contract convertor
var plutusContractConvertor = new PlutusContractConvertor(
    plutusDataModelConvertor,
    usersConvertor,
    processesConvertor);

var plutusContract = plutusContractConvertor.Convert(contract);

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
var contractCode = new PlutusGenerator().GeneratePlutusContract(plutusContract);
Console.WriteLine(contractCode.InString());
Console.WriteLine("COPIED TO CLIPBOARD");
new Clipboard().SetText(contractCode.InString());

