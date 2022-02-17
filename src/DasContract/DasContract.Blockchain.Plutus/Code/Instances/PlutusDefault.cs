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

        static IEnumerable<IPlutusLine> GetLinesOfCode(PlutusRecord record)
        {
            return GetDeclaration(record)
                .Concat(GetDefaultLines(record));
        }

        static IEnumerable<IPlutusLine> GetDefaultLines(PlutusRecord record)
        {
            var result = new List<IPlutusLine>
            {
                new PlutusRawLine(1, "def = " + record.Name + " {")
            };


            foreach (var member in record.Members)
            {
                if (member.DataType.Name == PlutusInteger.Type.Name)
                    result.Add(new PlutusRawLine(2, $"{member.Name} = 0"));
                else if (member.DataType.Name == PlutusPubKeyHash.Type.Name)
                    result.Add(new PlutusRawLine(2, $"{member.Name} = \"00000000000000000000000000000000000000000000000000000000\""));
                else if (member.DataType.Name == PlutusBool.Type.Name)
                    result.Add(new PlutusRawLine(2, $"{member.Name} = False"));
                else if (member.DataType.Name == PlutusByteString.Type.Name)
                    result.Add(new PlutusRawLine(2, $"{member.Name} = \"\""));
                else if (member.DataType.Name == PlutusPOSIXTime.Type.Name)
                    result.Add(new PlutusRawLine(2, $"{member.Name} = 0"));
                else if (member.DataType.Name.StartsWith("[") && member.DataType.Name.EndsWith("]"))
                    result.Add(new PlutusRawLine(2, $"{member.Name} = []"));
                else if (member.DataType.Name.StartsWith("Maybe "))
                    result.Add(new PlutusRawLine(2, $"{member.Name} = Nothing "));
                else
                    result.Add(new PlutusRawLine(2, $"{member.Name} = def"));
            }

            result.Add(new PlutusRawLine(1, "}"));
            return result;
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
