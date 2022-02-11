using System;
using System.Collections.Generic;
using System.Text;

namespace DasContract.Blockchain.Plutus.Code.Comments
{
    public class PlutusRawLine : PlutusLine
    {
        public PlutusRawLine(int indent, string code): base(indent)
        {
            Code = code;
        }

        public string Code { get; }

        /// <inheritdoc/>
        public override string InString()
        {
            if (string.IsNullOrWhiteSpace(Code))
                return string.Empty;
            return base.InString() + Code;
        }
    }
}
