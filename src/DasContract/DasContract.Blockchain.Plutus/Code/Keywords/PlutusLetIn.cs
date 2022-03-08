using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Blockchain.Plutus.Code.Comments;

namespace DasContract.Blockchain.Plutus.Code.Keywords
{
    public class PlutusLetIn : PlutusCode
    {
        public PlutusLetIn(int indent, IEnumerable<IPlutusLine> letCode, IEnumerable<IPlutusLine> inCode) 
            : base(GetLinesOfCode(indent, letCode, inCode))
        {
        }

        private static IEnumerable<IPlutusLine> GetLinesOfCode(int indent,
            IEnumerable<IPlutusLine> letCode, 
            IEnumerable<IPlutusLine> inCode)
        {
            return new List<IPlutusLine>()
                .Append(new PlutusRawLine(indent, "let"))
                .Concat(letCode)
                .Append(new PlutusRawLine(indent, "in"))
                .Concat(inCode);
        }
    }
}
