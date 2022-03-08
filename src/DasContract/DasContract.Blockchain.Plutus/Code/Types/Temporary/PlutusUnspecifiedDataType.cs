using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Code.Types.Temporary
{
    public class PlutusUnspecifiedDataType : INamable
    {
        public string Name { get; }

        public PlutusUnspecifiedDataType(string name)
        {
            Name = name;
        }

        public static PlutusUnspecifiedDataType Type(string name) => new PlutusUnspecifiedDataType(name);
    }
}
