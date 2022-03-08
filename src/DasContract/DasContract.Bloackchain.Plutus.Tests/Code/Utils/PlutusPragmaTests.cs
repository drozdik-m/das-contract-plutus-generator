using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;
using NUnit.Framework;

namespace DasContract.Bloackchain.Plutus.Tests.Code.Utils
{
    public class PlutusPragmaTests
    {

        [Test]
        public void PlutusPragma()
        {
            var line = new PlutusPragma(0, "pragma");
            Assert.AreEqual("{-# pragma #-}", line.InString());
        }

        [Test]
        public void PlutusPragmaIndent()
        {
            var line = new PlutusPragma(2, "pragma");
            Assert.AreEqual(PlutusLine.IndentString +
                PlutusLine.IndentString +
                "{-# pragma #-}", line.InString());
        }
    }
}
