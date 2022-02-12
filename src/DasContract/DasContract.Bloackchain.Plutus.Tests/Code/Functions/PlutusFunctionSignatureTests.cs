using System.Collections.Generic;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;
using NUnit.Framework;

namespace DasContract.Bloackchain.Plutus.Tests
{
    public class PlutusFunctionSignatureTests
    {
        class TestType: INamable
        {
            public TestType(string name)
            {
                Name = name;
            }

            public string Name { get; }
        }

        [Test]
        public void FunctionSignature()
        {
            var line = new PlutusFunctionSignature(0, "Function", new List<TestType>
            {
                new TestType("A"),
                new TestType("B"),
                new TestType("C"),
            });
            Assert.AreEqual("Function :: A -> B -> C", line.InString());
        }

        [Test]
        public void FunctionSignatureOneType()
        {
            var line = new PlutusFunctionSignature(0, "Function", new List<TestType>
            {
                new TestType("A")
            });
            Assert.AreEqual("Function :: A", line.InString());
        }

        [Test]
        public void FunctionSignatureNoTypes()
        {
            try
            {
                var line = new PlutusFunctionSignature(0, "Function", new List<TestType>());
                Assert.Fail();
            }
            catch { }
        }
    }
}
