using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Code.Types.Premade
{
    public class PlutusMaybe : PlutusPremadeType
    {
        

        public PlutusMaybe(INamable innerType)
        {
            InnerType = innerType;
        }

        public INamable InnerType { get; }

        public override string Name => $"Maybe {PlutusCode.ProperlyBracketed(InnerType.Name)}";

        public static PlutusMaybe Type(INamable innerType) => new PlutusMaybe(innerType);
    }
}
