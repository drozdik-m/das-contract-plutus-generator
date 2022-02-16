using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Code.Types.Premade
{
    public class PlutusPOSIXTime : PlutusPremadeType
    {
        public override string Name { get; } = "POSIXTime";

        public static PlutusPOSIXTime Type { get; } = new PlutusPOSIXTime();
    }
}
