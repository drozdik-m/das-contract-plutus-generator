using System.Collections.Generic;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;
using NUnit.Framework;

namespace DasContract.Bloackchain.Plutus.Tests
{
    public class PlutusFunctionTests
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
        public void PlutusFunction()
        {
            var signature = new PlutusFunctionSignature(0, "Function", new List<TestType>
            {
                new TestType("A"),
                new TestType("B"),
                new TestType("C"),
            });
            var line = new PlutusFunction(0, signature, new List<string>
            {
                "par1",
                "par2"
            }, new List<IPlutusLine> 
            { 
                new PlutusRawLine(1, "test line 1"),
                new PlutusRawLine(1, "test line 2"),
            });


            Assert.AreEqual("Function par1 par2 =" + PlutusCode.NewLineString +
                PlutusLine.IndentString + "test line 1" + PlutusCode.NewLineString +
                PlutusLine.IndentString + "test line 2" + PlutusCode.NewLineString
                , line.InString());
        }

        [Test]
        public void PlutusFunctionNoParams()
        {
            var signature = new PlutusFunctionSignature(0, "Function", new List<TestType>
            {
                new TestType("A"),
                new TestType("B"),
                new TestType("C"),
            });
            var line = new PlutusFunction(0, signature, new List<string>
            {

            }, new List<IPlutusLine>
            {
                new PlutusRawLine(1, "test line 1"),
                new PlutusRawLine(1, "test line 2"),
            });


            Assert.AreEqual("Function =" + PlutusCode.NewLineString +
                PlutusLine.IndentString + "test line 1" + PlutusCode.NewLineString +
                PlutusLine.IndentString + "test line 2" + PlutusCode.NewLineString
                , line.InString());
        }


        [Test]
        public void PlutusFunctionTooManyParams()
        {
            var signature = new PlutusFunctionSignature(0, "Function", new List<TestType>
            {
                new TestType("A"),
                new TestType("B"),
                new TestType("C"),
            });
            try
            {
                var line = new PlutusFunction(0, signature, new List<string>
                {
                    "par1",
                    "par2",
                    "par3",
                }, new List<IPlutusLine>
                {
                    new PlutusRawLine(1, "test line 1"),
                    new PlutusRawLine(1, "test line 2"),
                });
                Assert.Fail();
            }
            catch { }

        }

    }
}
