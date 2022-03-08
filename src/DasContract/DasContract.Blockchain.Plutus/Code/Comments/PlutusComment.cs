using System;
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
            if (string.IsNullOrWhiteSpace(Comment))
                return string.Empty;
            return base.InString() + $"-- {Comment}";
        }
    }
}
