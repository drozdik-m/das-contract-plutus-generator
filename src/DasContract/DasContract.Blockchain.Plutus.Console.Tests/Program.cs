using System.Reflection;
using System.Xml.Linq;
using DasContract.Abstraction;
using DasContract.Blockchain.Plutus;
using DasContract.Blockchain.Plutus.Console.Tests.DemoContracts;
using DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels;
using DasContract.Blockchain.Plutus.Data.Processes;
using DasContract.Blockchain.Plutus.Data.Processes.Process;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Events;

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
    referencePropertyConvertor,
    contractEnumConvertor);

//Contract convertor
var plutusContractConvertor = new PlutusContractConvertor(
    plutusDataModelConvertor);

var plutusContract = plutusContractConvertor.Convert(contract);
plutusContract.Processes = new ContractProcesses()
{
     AllProcesses = new ContractProcess[]
     {
         new ContractProcess()
         {
            Id = "Default process",
            IsMain = true,
            StartEvent = new ContractStartEvent()
            {
                Id = "Start",
                Outgoing = new ContractEndEvent()
                {
                    Id = "End"
                }
            }
         },

     }
};

//Generate
Console.WriteLine(new PlutusGenerator().GeneratePlutusContract(plutusContract));

