using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;
using NUnit.Framework;

namespace DasContract.Bloackchain.Plutus.Tests.Code.Comments
{
    public class PlutusSectionCommentTests
    {

        const string DIVIDER = "----------------------------------------------------";

        [Test]
        public void PlutusSectionComment()
        {
            var code = new PlutusSectionComment(0, "Hello there");
            Assert.AreEqual(DIVIDER + PlutusCode.NewLineString +
                "-- Hello there -------------------------------------" + PlutusCode.NewLineString +
                DIVIDER + PlutusCode.NewLineString,
                code.InString());
        }

        [Test]
        public void PlutusSectionCommentIndent()
        {
            var code = new PlutusSectionComment(2, "Hello there");
            Assert.AreEqual(PlutusLine.IndentString + PlutusLine.IndentString + DIVIDER + PlutusCode.NewLineString +
                PlutusLine.IndentString + PlutusLine.IndentString + "-- Hello there -------------------------------------" + PlutusCode.NewLineString +
                PlutusLine.IndentString + PlutusLine.IndentString + DIVIDER + PlutusCode.NewLineString,
                code.InString());
        }
    }
}
