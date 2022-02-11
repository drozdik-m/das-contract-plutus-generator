using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DasContract.Blockchain.Plutus.Code.Comments
{
    public class PlutusCommentDivider : PlutusLine
    {
        public PlutusCommentDivider(int indent): base(indent)
        {
            
        }

        /// <inheritdoc/>
        public override string InString()
        {
            return base.InString() + 
                string.Join(string.Empty, Enumerable.Repeat("-", Length));
        }

        public static int Length = 52;
    }
}
