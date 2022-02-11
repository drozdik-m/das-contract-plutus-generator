using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Blockchain.Plutus.Code.Comments;

namespace DasContract.Blockchain.Plutus.Code.Types
{
    public class PlutusRecord : PlutusCode
    {
        public PlutusRecord(string name, 
            IEnumerable<PlutusRecordMember> members, 
            IEnumerable<string> derivings)
            :base(GetLinesOfCode(name, members, derivings))
        {

        }

        static IEnumerable<IPlutusLine> GetLinesOfCode(string name, 
            IEnumerable<PlutusRecordMember> members,
            IEnumerable<string> derivings)
        {
            //Members
            var updatedMembers = members;
            if (updatedMembers.Count() > 0)
            {
                var last = updatedMembers.Last();
                updatedMembers = updatedMembers
                                    .SkipLast(1)
                                    .Append(last.ToLast());
            }

            //Derivings
            var derivingsString = string.Empty;
            if (derivings.Count() > 0)
                derivingsString = " deriving (" + string.Join(", ", derivings) + ")";

            return new List<IPlutusLine>()
                .Append(new PlutusRawLine(0, $"data {name} = {name} " + "{"))
                .Concat(updatedMembers)
                .Append(new PlutusRawLine(0, "}" + derivingsString));
        }
    }
}
