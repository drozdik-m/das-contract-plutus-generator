using System.Collections.Generic;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using NUnit.Framework;

namespace DasContract.Bloackchain.Plutus.Tests.Code.Functions
{
    public class PlutusGuardFunctionTests
    {
        class TestType : INamable
        {
            public TestType(string name)
            {
                Name = name;
            }

            public string Name { get; }
        }

        [Test]
        public void PlutusGuardFunction()
        {
            var signature = new PlutusFunctionSignature(0, "Function", new List<TestType>
            {
                new TestType("A"),
                new TestType("B"),
                new TestType("C"),
            });
            var line = new PlutusGuardFunction(0, signature, new List<string>
            {
                "par1",
                "par2"
            }, new List<(string, string)>
            {
                ("i <= 0", "LoopEnded"),
                ("otherwise", "ToLoop i")
            });


            Assert.AreEqual("Function par1 par2" + PlutusCode.NewLineString +
                PlutusLine.IndentString + "| i <= 0 = LoopEnded" + PlutusCode.NewLineString +
                PlutusLine.IndentString + "| otherwise = ToLoop i" + PlutusCode.NewLineString
                , line.InString());
        }

        [Test]
        public void PlutusGuardFunctionNoParams()
        {
            var signature = new PlutusFunctionSignature(0, "Function", new List<TestType>
            {
                new TestType("A"),
                new TestType("B"),
                new TestType("C"),
            });
            var line = new PlutusGuardFunction(0, signature, new List<string>
            {

            }, new List<(string, string)>
            {
                ("i <= 0", "LoopEnded"),
                ("otherwise", "ToLoop i")
            });


            Assert.AreEqual("Function" + PlutusCode.NewLineString +
                PlutusLine.IndentString + "| i <= 0 = LoopEnded" + PlutusCode.NewLineString +
                PlutusLine.IndentString + "| otherwise = ToLoop i" + PlutusCode.NewLineString
                , line.InString());
        }

        [Test]
        public void PlutusGuardFunctionTooManyParams()
        {
            var signature = new PlutusFunctionSignature(0, "Function", new List<TestType>
            {
                new TestType("A"),
                new TestType("B"),
                new TestType("C"),
            });
            try
            {
                var line = new PlutusGuardFunction(0, signature, new List<string>
                {
                    "par1",
                    "par2",
                    "par3"
                }, new List<(string, string)>
                {
                    ("i <= 0", "LoopEnded"),
                    ("otherwise", "ToLoop i")
                });
                Assert.Fail();
            }
            catch { }

        }

    }
}
