using System;
using System.Collections.Generic;
using System.Text;

namespace DasContract.Blockchain.Plutus.Code.Comments
{
    public class PlutusImport : PlutusLine
    {
        public PlutusImport(int indent, string content): base(indent)
        {
            Content = content;
        }

        public string Content { get; }

        /// <inheritdoc/>
        public override string InString()
        {
            if (string.IsNullOrWhiteSpace(Content))
                return string.Empty;
            return base.InString() + "import " + Content;
        }
    }
}
