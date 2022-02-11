using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Blockchain.Plutus.Code.Comments;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Code.Types
{
    public class PlutusAlgebraicType : PlutusCode, INamable
    {
        public PlutusAlgebraicType(string name, 
            IEnumerable<PlutusAlgebraicTypeConstructor> constructors, 
            IEnumerable<string> derivings)
            :base(GetLinesOfCode(name, constructors, derivings))
        {
            Name = name;
            Constructors = constructors;
            Derivings = derivings;
        }

        public string Name { get; }

        public IEnumerable<PlutusAlgebraicTypeConstructor> Constructors { get; }

        public IEnumerable<string> Derivings { get; }

        static IEnumerable<IPlutusLine> GetLinesOfCode(string name, 
            IEnumerable<PlutusAlgebraicTypeConstructor> constructors,
            IEnumerable<string> derivings)
        {
            //Constructors
            if (constructors.Count() == 0)
                throw new Exception("It is not allowed to have a data type without a constructor");

            var last = constructors.Last();
            constructors = constructors
                                .SkipLast(1)
                                .Append(last.ToLast());

            //Derivings
            var derivingsString = string.Empty;
            if (derivings.Count() > 0)
                derivingsString = "  deriving (" + string.Join(", ", derivings) + ")";

            //Keyword
            var keyword = constructors.Count() == 1 ? "newtype" : "data";

            var result = new List<IPlutusLine>()
                .Append(new PlutusRawLine(0, $"{keyword} {name} ="))
                .Concat(constructors);

            if (string.IsNullOrEmpty(derivingsString))
                return result;
            return result.Append(new PlutusRawLine(0, derivingsString));
                
                
        }
    }
}
