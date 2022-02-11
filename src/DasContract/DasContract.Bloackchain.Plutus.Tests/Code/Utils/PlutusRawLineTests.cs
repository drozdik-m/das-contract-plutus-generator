using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;
using NUnit.Framework;

namespace DasContract.Bloackchain.Plutus.Tests
{
    public class PlutusRawLineTests
    {

        [Test]
        public void PlutusRawCode()
        {
            var line = new PlutusRawLine(0, "stuff");
            Assert.AreEqual("stuff", line.InString());
        }

        [Test]
        public void PlutusRawLineIndent()
        {
            var line = new PlutusRawLine(2, "stuff");
            Assert.AreEqual(PlutusLine.IndentString + 
                PlutusLine.IndentString + 
                "stuff", line.InString());
        }
    }
}
