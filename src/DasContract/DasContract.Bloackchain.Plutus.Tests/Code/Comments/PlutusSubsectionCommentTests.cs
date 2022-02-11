using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;
using NUnit.Framework;

namespace DasContract.Bloackchain.Plutus.Tests
{
    public class PlutusSubsectionCommentTests
    {
        [Test]
        public void PlutusSubsectionComment()
        {
            var line = new PlutusSubsectionComment(0, "Hello there");
            Assert.AreEqual("-- Hello there -------------------------------------", 
                line.InString());
        }

        [Test]
        public void PlutusCommentDividerIndent()
        {
            var line = new PlutusSubsectionComment(2, "Hello there");
            Assert.AreEqual(PlutusLine.IndentString +
                PlutusLine.IndentString + 
                "-- Hello there -------------------------------------",
                line.InString());
        }
    }
}
