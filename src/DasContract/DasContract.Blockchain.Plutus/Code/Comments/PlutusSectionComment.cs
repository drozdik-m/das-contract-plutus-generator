using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DasContract.Blockchain.Plutus.Code.Comments
{
    public class PlutusSectionComment : PlutusCode
    {
        public PlutusSectionComment(int indent, string comment): base(
                new List<IPlutusLine>()
                {
                    new PlutusCommentDivider(indent),
                    new PlutusSubsectionComment(indent, comment),
                    new PlutusCommentDivider(indent)
                }
            )
        {
            
        }

    }
}
