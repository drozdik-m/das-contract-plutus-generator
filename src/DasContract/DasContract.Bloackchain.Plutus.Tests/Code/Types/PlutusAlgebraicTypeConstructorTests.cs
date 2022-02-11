using System;
using System.Collections.Generic;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;
using DasContract.Blockchain.Plutus.Code.Types;
using NUnit.Framework;

namespace DasContract.Bloackchain.Plutus.Tests
{
    public class PlutusAlgebraicTypeConstructorTests
    {
        [Test]
        public void PlutusAlgebraicTypeConstructor()
        {
            var member = new PlutusAlgebraicTypeConstructor("A", new List<string> { "Type" }, false);
            Assert.AreEqual(PlutusLine.IndentString +
                "A Type |", member.InString());
        }

        [Test]
        public void PlutusAlgebraicTypeConstructorLast()
        {
            var member = new PlutusAlgebraicTypeConstructor("A", new List<string> { "Type" }, true);
            Assert.AreEqual(PlutusLine.IndentString +
                "A Type", member.InString());
        }

        [Test]
        public void PlutusAlgebraicTypeConstructorNoConstructors()
        {
            var member = new PlutusAlgebraicTypeConstructor("A", new List<string>());
            Assert.AreEqual(PlutusLine.IndentString +
                "A |", member.InString());
        }

        [Test]
        public void PlutusAlgebraicTypeConstructorEmptyName()
        {
            try
            {
                var member = new PlutusAlgebraicTypeConstructor("", new List<string> { "Type" }, true);
                Assert.Fail();
            }
            catch
            {

            }
        }
    }
}
