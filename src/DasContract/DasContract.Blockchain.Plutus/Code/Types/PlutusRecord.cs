using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Blockchain.Plutus.Code.Comments;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Code.Types
{
    public class PlutusRecord : PlutusCode, INamable
    {
        public PlutusRecord(string name, 
            IEnumerable<PlutusRecordMember> members, 
            IEnumerable<string> derivings)
            :base(GetLinesOfCode(name, members, derivings))
        {
            Name = name;
            Members = members;
            Derivings = derivings;
        }

        public string Name { get; }

        public IEnumerable<PlutusRecordMember> Members { get; }

        public IEnumerable<string> Derivings { get; }

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

            //Keyword
            var keyword = members.Count() == 1 ? "newtype" : "data";

            return new List<IPlutusLine>()
                .Append(new PlutusRawLine(0, $"{keyword} {name} = {name} " + "{"))
                .Concat(updatedMembers)
                .Append(new PlutusRawLine(0, "}" + derivingsString));
        }
    }
}
