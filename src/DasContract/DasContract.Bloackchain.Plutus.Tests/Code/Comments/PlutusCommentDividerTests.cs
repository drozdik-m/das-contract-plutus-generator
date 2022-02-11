using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;
using NUnit.Framework;

namespace DasContract.Bloackchain.Plutus.Tests
{
    public class PlutusCommentDividerTests
    {

        const string DIVIDER = "----------------------------------------------------";

        [Test]
        public void PlutusCommentDivider()
        {
            var line = new PlutusCommentDivider(0);
            Assert.AreEqual(DIVIDER, 
                line.InString());
        }

        [Test]
        public void PlutusCommentDividerIndent()
        {
            var line = new PlutusCommentDivider(2);
            Assert.AreEqual(PlutusLine.IndentString + 
                PlutusLine.IndentString +
                DIVIDER, line.InString());
        }
    }
}
