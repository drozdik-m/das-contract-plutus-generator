using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Code.Types.Premade
{
    public class PlutusDictionary : PlutusPremadeType
    {
        
        public PlutusDictionary(INamable key, INamable value)
        {
            Key = key;
            Value = value;
        }

        public override string Name => $"[({Key.Name}, {Value.Name})]";

        public INamable Key { get; }
        public INamable Value { get; }

        public static PlutusDictionary Type(INamable key, INamable value) 
            => new PlutusDictionary(key, value);
    }
}
