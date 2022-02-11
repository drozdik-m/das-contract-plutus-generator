using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DasContract.Blockchain.Plutus.Code
{
    public class PlutusCode : IPlutusCode
    {
        public IEnumerable<IPlutusLine> LinesOfCode { get; }

        public PlutusCode(IEnumerable<IPlutusLine> linesOfCode)
        {
            LinesOfCode = linesOfCode;
        }

        /// <inheritdoc/>
        public IPlutusCode Append(IPlutusCode code)
        {
            return new PlutusCode(LinesOfCode.Concat(code.LinesOfCode));
        }

        /// <inheritdoc/>
        public IPlutusCode Prepend(IPlutusCode code)
        {
            return new PlutusCode(code.LinesOfCode.Concat(LinesOfCode));
        }

        /// <inheritdoc/>
        public string InString()
        {
            return LinesOfCode.Aggregate("", (acc, line) => acc + line.InString() + NewLineString);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return InString();
        }

        public static string NewLineString = Environment.NewLine;
    }
}
