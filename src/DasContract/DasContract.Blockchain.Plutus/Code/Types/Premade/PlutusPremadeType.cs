using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Code.Types.Premade
{
    public abstract class PlutusPremadeType : INamable
    {
        public abstract string Name { get; }
    }
}
