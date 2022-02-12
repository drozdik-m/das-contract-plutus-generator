using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DasContract.Blockchain.Plutus.Code;

namespace DasContract.Bloackchain.Plutus.Tests.Code.Types.Premade
{
    public class PlutusInteger : INamable
    {
        public string Name { get; } = "Integer";

        public static PlutusInteger Type { get; } = new PlutusInteger();
    }
}
