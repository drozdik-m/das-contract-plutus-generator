using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Code.Comments
{
    public class PlutusUnstableMakeIsData : PlutusLine
    {
        public PlutusUnstableMakeIsData(INamable item): base(0)
        {
            Item = item;
        }

        public INamable Item { get; }

        /// <inheritdoc/>
        public override string InString()
        {
            return base.InString() + $"PlutusTx.unstableMakeIsData ''{Item.Name}";
        }
    }
}
