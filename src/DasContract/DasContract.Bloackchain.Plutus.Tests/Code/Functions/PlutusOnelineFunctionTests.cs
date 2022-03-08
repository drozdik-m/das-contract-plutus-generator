using System.Collections.Generic;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using NUnit.Framework;

namespace DasContract.Bloackchain.Plutus.Tests.Code.Functions
{
    public class PlutusOnelineFunctionTests
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
        public void PlutusOnelineFunction()
        {
            var signature = new PlutusFunctionSignature(0, "Function", new List<TestType>
            {
                new TestType("A"),
                new TestType("B"),
                new TestType("C"),
            });
            var line = new PlutusOnelineFunction(0, signature, new List<string>
            {
                "par1",
                "par2"
            }, "this is some code ma man");


            Assert.AreEqual("Function par1 par2 = this is some code ma man", line.InString());
        }

        [Test]
        public void PlutusOnelineFunctionNoParams()
        {
            var signature = new PlutusFunctionSignature(0, "Function", new List<TestType>
            {
                new TestType("A"),
                new TestType("B"),
                new TestType("C"),
            });
            var line = new PlutusOnelineFunction(0, signature, new List<string>
            {

            }, "this is some code ma man");


            Assert.AreEqual("Function = this is some code ma man", line.InString());
        }

        [Test]
        public void PlutusOnelineFunctionTooManyParams()
        {
            var signature = new PlutusFunctionSignature(0, "Function", new List<TestType>
            {
                new TestType("A"),
                new TestType("B"),
                new TestType("C"),
            });
            try
            {
                var line = new PlutusOnelineFunction(0, signature, new List<string>
                {
                    "par1",
                    "par2",
                    "par3",
                }, "this is some code ma man");
                Assert.Fail();
            }
            catch { }

        }

    }
}
