﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DasContract.Blockchain.Plutus.Code.Comments
{
    public class PlutusComment : PlutusLine
    {
        public PlutusComment(int indent, string comment): base(indent)
        {
            Comment = comment;
        }

        public string Comment { get; }

        /// <inheritdoc/>
        public override string InString()
        {
            return base.InString() + $"-- {Comment}";
        }
    }
}
