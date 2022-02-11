using System;
using System.Collections.Generic;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;
using DasContract.Blockchain.Plutus.Code.Types;
using NUnit.Framework;

namespace DasContract.Bloackchain.Plutus.Tests
{
    public class PlutusAlgebraicTypeTests
    {
        [Test]
        public void PlutusAlgebraicType()
        {
            var record = new PlutusAlgebraicType("Record", new List<PlutusAlgebraicTypeConstructor>()
            {
                new PlutusAlgebraicTypeConstructor("A", new List<string>{ "Type" }),
                new PlutusAlgebraicTypeConstructor("B", new List<string>()),
            }, new List<string>());

            Assert.AreEqual("data Record =" + PlutusCode.NewLineString +
                PlutusLine.IndentString + "A Type |" + PlutusCode.NewLineString +
                PlutusLine.IndentString + "B" + PlutusCode.NewLineString
                , record.InString());
        }

        [Test]
        public void PlutusAlgebraicTypeDerivings()
        {
            var record = new PlutusAlgebraicType("Record", new List<PlutusAlgebraicTypeConstructor>()
            {
                new PlutusAlgebraicTypeConstructor("A", new List<string>{ "Type" }),
                new PlutusAlgebraicTypeConstructor("B", new List<string>()),
            }, new List<string>
            {
                "Show", 
                "Generic"
            });

            Assert.AreEqual("data Record =" + PlutusCode.NewLineString +
                PlutusLine.IndentString + "A Type |" + PlutusCode.NewLineString +
                PlutusLine.IndentString + "B" + PlutusCode.NewLineString +
                "  deriving (Show, Generic)" + PlutusCode.NewLineString,
                record.InString());
        }

        [Test]
        public void PlutusAlgebraicTypeNoCtors()
        {
            try
            {
                var record = new PlutusAlgebraicType("Record", 
                    new List<PlutusAlgebraicTypeConstructor>(), 
                    new List<string>());

                Assert.Fail();
            }
            catch { }


        }
    }
}
