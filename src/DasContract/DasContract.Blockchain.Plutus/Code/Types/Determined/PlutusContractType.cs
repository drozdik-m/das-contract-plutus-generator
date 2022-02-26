using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Code.Types.Determined
{
    public class PlutusContractType : PlutusPremadeType
    {
        public override string Name { get; } = "ContractType";

        public static PlutusContractType Type { get; } = new PlutusContractType();
    }
}
