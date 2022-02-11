using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;
using NUnit.Framework;

namespace DasContract.Bloackchain.Plutus.Tests
{
    public class PlutusMakeLiftTests
    {
        class NamableItem : INamable
        {
            public string Name { get; set; } = "ItemName";
        }

        [Test]
        public void MakeLift()
        {
            Assert.AreEqual
                (
                    "PlutusTx.makeLift ''ItemName",
                    new PlutusMakeLift(new NamableItem()).InString()
                );
        }
    }
}
