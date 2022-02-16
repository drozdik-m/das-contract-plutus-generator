using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;
using NUnit.Framework;

namespace DasContract.Bloackchain.Plutus.Tests.Code.Comments
{
    public class PlutusCommentTests
    {

        [Test]
        public void PlutusComment()
        {
            var line = new PlutusComment(0, "Hello there");
            Assert.AreEqual("-- Hello there", line.InString());
        }

        [Test]
        public void PlutusCommentEmpty()
        {
            var line = new PlutusComment(0, "");
            Assert.AreEqual(string.Empty, line.InString());
        }

        [Test]
        public void PlutusCommentIndent()
        {
            var line = new PlutusComment(2, "Hello there");
            Assert.AreEqual(PlutusLine.IndentString +
                PlutusLine.IndentString +
                "-- Hello there", line.InString());
        }
    }
}
