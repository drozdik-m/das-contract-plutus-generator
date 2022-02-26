using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Blockchain.Plutus.Code.Comments;
using DasContract.Blockchain.Plutus.Code.Types.Premade;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using DasContract.Blockchain.Plutus.Utils;

namespace DasContract.Blockchain.Plutus.Code.Types
{
    public class PlutusDefault : PlutusCode
    {
        public PlutusDefault(PlutusRecord record)
            :base(GetLinesOfCode(record))
        {
            
        }

        public PlutusDefault(PlutusAlgebraicType algType, PlutusAlgebraicTypeConstructor defaultCtor)
            : base(GetLinesOfCode(algType, defaultCtor))
        {

        }

        static IEnumerable<IPlutusLine> GetLinesOfCode(PlutusRecord record)
        {
            return GetDeclaration(record)
                .Concat(GetDefaultLines(record));
        }

        static IEnumerable<IPlutusLine> GetLinesOfCode(PlutusAlgebraicType algType, PlutusAlgebraicTypeConstructor defaultCtor)
        {
            return GetDeclaration(algType)
                .Append(GetDefaultLine(algType, defaultCtor));
        }

        static IPlutusLine GetDefaultLine(PlutusAlgebraicType algType, PlutusAlgebraicTypeConstructor defaultCtor)
        {
            return new PlutusRawLine(1, $"def = {defaultCtor.Name} " +
                $"{string.Join(" ", defaultCtor.Types.Select(e => GetDefValueFor(e)))}");
        }

        static IEnumerable<IPlutusLine> GetDefaultLines(PlutusRecord record)
        {
            var result = new List<IPlutusLine>
            {
                new PlutusRawLine(1, "def = " + record.Name + " {")
            };

            var memberCount = record.Members.Count();
            var i = 0;

            foreach (var member in record.Members)
            {
                var lineEnding = ",";
                if (i == memberCount - 1)
                    lineEnding = "";

                result.Add(new PlutusRawLine(2, $"{member.Name} = {GetDefValueFor(member.DataType)}{lineEnding}"));

                i++;
            }

            result.Add(new PlutusRawLine(1, "}"));
            return result;
        }

        static string GetDefValueFor(INamable type)
        {
            if (type.Name == PlutusInteger.Type.Name)
                return "0";
            else if (type.Name == PlutusPubKeyHash.Type.Name)
                return "\"00000000000000000000000000000000000000000000000000000000\"";
            else if (type.Name == PlutusBool.Type.Name)
                return "False";
            else if (type.Name == PlutusBuiltinByteString.Type.Name)
                return "\"\"";
            else if (type.Name == PlutusPOSIXTime.Type.Name)
                return "0";
            else if (type.Name.StartsWith("[") && type.Name.EndsWith("]"))
                return "[]";
            else if (type.Name.StartsWith("Maybe "))
                return "Nothing";
            else
                return "def";
        }

        static IEnumerable<IPlutusLine> GetDeclaration(INamable item)
        {
            return new List<IPlutusLine>()
            {
                new PlutusRawLine(0, $"instance Default {item.Name} where"),
                new PlutusPragma(1, "INLINABLE def")
            };
        }

    }
}
