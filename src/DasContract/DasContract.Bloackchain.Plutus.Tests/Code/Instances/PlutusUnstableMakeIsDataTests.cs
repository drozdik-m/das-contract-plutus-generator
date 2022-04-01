using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;
using DasContract.Blockchain.Plutus.Code.Instances;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using NUnit.Framework;

namespace DasContract.Bloackchain.Plutus.Tests.Code.Instances
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
