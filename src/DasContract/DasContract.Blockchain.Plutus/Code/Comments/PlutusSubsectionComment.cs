using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DasContract.Blockchain.Plutus.Code.Comments
{
    public class PlutusSubsectionComment : PlutusLine
    {
        public PlutusSubsectionComment(int indent, string comment): base(indent)
        {
            Comment = comment;
        }

        public string Comment { get; }

        /// <inheritdoc/>
        public override string InString()
        {
            var beginWith = $"-- {Comment} ";
            return base.InString() +
                beginWith +
                Enumerable.Repeat("-", DashLimit - beginWith.Length);
        }

        public static int DashLimit = PlutusCommentDivider.Length;
    }
}
