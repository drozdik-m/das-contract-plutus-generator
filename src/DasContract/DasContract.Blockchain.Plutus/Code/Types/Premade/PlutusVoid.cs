using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Code.Types.Premade
{
    public class PlutusVoid : PlutusPremadeType
    {
        public override string Name { get; } = "()";

        public static PlutusVoid Type { get; } = new PlutusVoid();
    }
}
