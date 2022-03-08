using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Code.Types.Premade
{
    public class PlutusTuple : PlutusPremadeType
    {
        public PlutusTuple(INamable first, INamable second)
        {
            First = first;
            Second = second;
        }

        public INamable First { get; }

        public INamable Second { get; }

        public override string Name => $"({First.Name}, {Second.Name})";

        public static PlutusTuple Type(INamable first, INamable second) 
            => new PlutusTuple(first, second);
        
    }
}
