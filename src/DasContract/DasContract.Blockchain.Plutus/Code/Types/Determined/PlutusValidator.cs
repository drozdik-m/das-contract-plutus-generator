using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Code.Types.Determined
{
    public class PlutusValidator : PlutusPremadeType
    {
        public override string Name { get; } = "Validator";

        public static PlutusValidator Type { get; } = new PlutusValidator();
    }
}
