using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Code.Types.Premade
{
    public class PlutusList : PlutusPremadeType
    {
        
        public PlutusList(INamable innerType)
        {
            InnerType = innerType;
        }

        public INamable InnerType { get; }

        public override string Name => $"[{InnerType.Name}]";

        public static PlutusList Type(INamable innerType) => new PlutusList(innerType);
    }
}
