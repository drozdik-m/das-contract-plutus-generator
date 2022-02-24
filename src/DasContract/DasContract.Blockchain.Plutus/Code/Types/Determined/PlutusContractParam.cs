using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Code.Types.Determined
{
    public class PlutusContractParam : PlutusPremadeType
    {
        public override string Name { get; } = "ContractParam";

        public static PlutusContractParam Type { get; } = new PlutusContractParam();
    }
}
