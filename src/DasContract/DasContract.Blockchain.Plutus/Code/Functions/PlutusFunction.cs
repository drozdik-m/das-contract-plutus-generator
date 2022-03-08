using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DasContract.Blockchain.Plutus.Code.Comments
{
    /// <summary>
    /// Regular plutus function with a signature, parameters and code
    /// </summary>
    public class PlutusFunction : PlutusCode
    {
        public PlutusFunction(int indent, 
            PlutusFunctionSignature signature, 
            IEnumerable<string> parameterNames,
            IEnumerable<IPlutusLine> codeLines)
            : base(GetLinesOfCode(indent, signature, parameterNames, codeLines))
        {
            if (parameterNames.Count() > signature.Types.Count() - 1)
                throw new ArgumentException("Too many parameter names");
            Indent = indent;
            Signature = signature;
            ParameterNames = parameterNames;
            CodeLines = codeLines;
        }

        public int Indent { get; }

        public PlutusFunctionSignature Signature { get; }

        public IEnumerable<string> ParameterNames { get; }

        public IEnumerable<IPlutusLine> CodeLines { get; }

        public static IEnumerable<IPlutusLine> GetLinesOfCode(int indent,
            PlutusFunctionSignature signature, 
            IEnumerable<string> parameterNames,
            IEnumerable<IPlutusLine> codeLines)
        {
            var paramNames = parameterNames.Aggregate("", (acc, item) => acc + item + " ");
            var firstLine = new PlutusRawLine(indent, $"{signature.Name} {paramNames}=");

            return codeLines.Prepend(firstLine);
        }
    }
}
