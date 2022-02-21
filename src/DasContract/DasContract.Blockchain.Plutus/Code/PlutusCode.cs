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
            return new PlutusCodes(new List<IPlutusCode>()
            {
                this,
                code
            });
        }

        /// <inheritdoc/>
        public IPlutusCode Append(IPlutusLine line)
        {
            return new PlutusCodes(new List<IPlutusCode>()
            {
                this,
                new PlutusCode(new List<IPlutusLine>(){ line })
            });
        }

        /// <inheritdoc/>
        public IPlutusCode Prepend(IPlutusCode code)
        {
            return new PlutusCodes(new List<IPlutusCode>()
            {
                code,
                this
            });
        }

        /// <inheritdoc/>
        public IPlutusCode Prepend(IPlutusLine line)
        {
            return new PlutusCodes(new List<IPlutusCode>()
            {
                new PlutusCode(new List<IPlutusLine>(){ line }),
                this
            });
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

        public static PlutusCode Empty => new PlutusCode(Array.Empty<IPlutusLine>());

        public static string NewLineString = Environment.NewLine;
    }
}
