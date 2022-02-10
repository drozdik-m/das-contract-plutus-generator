using System;
using System.Collections.Generic;
using System.Text;

namespace DasContract.Blockchain.Plutus
{
    public class PlutusCode
    {
        public PlutusCode(string theCode)
        {
            TheCode = theCode;
        }

        public string TheCode { get; }

        public override string ToString()
        {
            return TheCode;
        }
    }
}
