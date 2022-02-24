﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Code.Types.Premade
{
    public class PlutusState : PlutusPremadeType
    {
        

        public PlutusState(INamable innerType)
        {
            InnerType = innerType;
        }

        public INamable InnerType { get; }

        private string InnerTypeString => InnerType.Name.Contains(" ")
            ? $"({InnerType.Name})"
            : InnerType.Name;

        public override string Name => $"State {InnerTypeString}";

        public static PlutusState Type(INamable innerType) => new PlutusState(innerType);
    }
}
