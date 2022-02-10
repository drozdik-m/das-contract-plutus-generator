using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DasContract.Blockchain.Plutus.Code
{
    public class PlutusLine : IPlutusLine
    {
        public int Indent { get; }

        public PlutusLine(int indent)
        {
            Indent = indent;
        }

        /// <inheritdoc/>
        public virtual string InString()
        {
            return Enumerable.Repeat(IndentString, Indent).ToString();
        }

        /// <summary>
        /// String for one level of indentation
        /// </summary>
        public static string IndentString = "\t";
    }
}
