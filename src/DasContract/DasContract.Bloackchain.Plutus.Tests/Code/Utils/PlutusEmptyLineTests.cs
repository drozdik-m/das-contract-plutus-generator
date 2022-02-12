using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;
using NUnit.Framework;

namespace DasContract.Bloackchain.Plutus.Tests
{
    public class PlutusEmptyLineTests
    {

        [Test]
        public void PlutusEmptyLine()
        {
            var line = PlutusLine.Empty;
            Assert.AreEqual(string.Empty, line.InString());
        }

    }
}
