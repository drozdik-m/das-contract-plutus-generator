using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DasContract.Blockchain.Plutus.Code
{
    public class PlutusCodes : IPlutusCode
    {
        public IEnumerable<IPlutusCode> Codes { get; }

        public PlutusCodes(IEnumerable<IPlutusCode> codes)
        {
            Codes = codes;
        }

        /// <inheritdoc/>
        public IPlutusCode Append(IPlutusCode code)
        {
            return new PlutusCodes(Codes.Append(code));
        }

        /// <inheritdoc/>
        public IPlutusCode Prepend(IPlutusCode code)
        {
            return new PlutusCodes(Codes.Prepend(code));
        }

        /// <inheritdoc/>
        public string InString()
        {
            return Codes.Aggregate("", (acc, code) => acc + code.InString());
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return InString();
        }
    }
}
