using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Blockchain.Plutus.Code.Comments;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using DasContract.Blockchain.Plutus.Utils;

namespace DasContract.Blockchain.Plutus.Code.Types
{
    public class PlutusEq : PlutusCode
    {
        public PlutusEq(PlutusRecord record)
            :base(GetLinesOfCode(record))
        {
            
        }

        static IEnumerable<IPlutusLine> GetLinesOfCode(PlutusRecord record)
        {
            var generator = new StableVariableNameGenerator();
            var leftSide = $"{record.Name} ";
            var rightSide = $"{record.Name} ";
            var comparison = "";
            foreach (var member in record.Members)
            {
                
            }
        }

        static IEnumerable<IPlutusLine> GetDeclaration(INamable item)
        {
            return new List<IPlutusLine>()
            {
                new PlutusRawLine(0, $"instance Eq {item.Name} where"),
                new PlutusPragma(1, "INLINABLE (==)")
            };
        }

        static IEnumerable<IPlutusLine> GetOtherwiseLine()
        {
            return new List<IPlutusLine>()
            {
                new PlutusRawLine(0, $"_ == _ = False"),
            };
        }
    }
}
