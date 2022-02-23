using DasContract.Blockchain.Plutus;
using DasContract.Blockchain.Plutus.Console.Tests.DemoContracts;

var contract = TransitionDemoContract.Get();
Console.WriteLine(new PlutusGenerator().GeneratePlutusContract(contract));
