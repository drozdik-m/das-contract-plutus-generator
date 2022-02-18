using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Blockchain.Plutus.Code.Comments;

namespace DasContract.Blockchain.Plutus.Code.Keywords
{
    public class PlutusWhere : PlutusCode
    {
        public PlutusWhere(int indent, IEnumerable<IPlutusLine> code, IEnumerable<IPlutusLine> whereCode) 
            : base(GetLinesOfCode(indent, code, whereCode))
        {
        }

        private static IEnumerable<IPlutusLine> GetLinesOfCode(int indent,
            IEnumerable<IPlutusLine> code, 
            IEnumerable<IPlutusLine> whereCode)
        {
            return new List<IPlutusLine>()
                .Concat(code)
                .Append(new PlutusRawLine(indent, "where"))
                .Concat(whereCode);
        }
    }
}
