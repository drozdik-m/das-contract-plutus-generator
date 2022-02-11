using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;
using NUnit.Framework;

namespace DasContract.Bloackchain.Plutus.Tests
{
    public class PlutusUnstableMakeIsDataTests
    {
        class NamableItem : INamable
        {
            public string Name { get; set; } = "ItemName";
        }

        [Test]
        public void PlutusUnstableMakeIsData()
        {
            Assert.AreEqual
                (
                    "PlutusTx.unstableMakeIsData ''ItemName",
                    new PlutusUnstableMakeIsData(new NamableItem()).InString()
                );
        }
    }
}
