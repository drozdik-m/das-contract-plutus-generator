using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;
using NUnit.Framework;

namespace DasContract.Bloackchain.Plutus.Tests.Code.Utils
{
    public class PlutusImportTests
    {

        [Test]
        public void PlutusImport()
        {
            var line = new PlutusImport(0, "stuff");
            Assert.AreEqual("import stuff", line.InString());
        }

        [Test]
        public void PlutusImportIndent()
        {
            var line = new PlutusImport(2, "stuff");
            Assert.AreEqual(PlutusLine.IndentString +
                PlutusLine.IndentString +
                "import stuff", line.InString());
        }
    }
}
