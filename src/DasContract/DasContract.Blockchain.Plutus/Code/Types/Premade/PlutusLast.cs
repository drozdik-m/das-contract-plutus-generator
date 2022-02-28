using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Code.Types.Premade
{
    public class PlutusLast : PlutusPremadeType
    {
        
        public PlutusLast(INamable innerType)
        {
            InnerType = innerType;
        }

        public INamable InnerType { get; }

        public override string Name => $"(Last {InnerType.Name})";

        public static PlutusLast Type(INamable innerType) => new PlutusLast(innerType);
    }
}
