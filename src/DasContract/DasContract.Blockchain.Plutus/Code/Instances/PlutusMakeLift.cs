﻿using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Code.Instances
{
    /// <summary>
    /// Instance for makeLift of an entity
    /// </summary>
    public class PlutusMakeLift : PlutusLine
    {
        public PlutusMakeLift(INamable item) : base(0)
        {
            Item = item;
        }

        public INamable Item { get; }

        /// <inheritdoc/>
        public override string InString()
        {
            return base.InString() + $"PlutusTx.makeLift ''{Item.Name}";
        }
    }
}
