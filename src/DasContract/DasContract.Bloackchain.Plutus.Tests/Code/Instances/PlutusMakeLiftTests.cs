using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using NUnit.Framework;

namespace DasContract.Bloackchain.Plutus.Tests.Code.Instances
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
