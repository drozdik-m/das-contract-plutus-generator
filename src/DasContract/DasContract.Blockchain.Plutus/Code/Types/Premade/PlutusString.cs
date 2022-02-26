using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Code.Types.Premade
{
    public class PlutusString : PlutusPremadeType
    {
        public override string Name { get; } = "String";

        public static PlutusString Type { get; } = new PlutusString();
    }
}
