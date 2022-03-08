using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Code.Types.Temporary
{
    public class PlutusFutureDataType : INamable
    {
        public string Name { get; }

        public PlutusFutureDataType(string name)
        {
            Name = name;
        }

        public static PlutusFutureDataType Type(string name) => new PlutusFutureDataType(name);
    }
}
