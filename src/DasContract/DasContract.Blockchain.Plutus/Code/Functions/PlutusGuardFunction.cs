using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DasContract.Blockchain.Plutus.Code.Comments
{
    public class PlutusGuardFunction : PlutusCode
    {
        public PlutusGuardFunction(int indent, 
            PlutusFunctionSignature signature, 
            IEnumerable<string> parameterNames,
            IEnumerable<(string, string)> conditionsAndCodes)
            : base(GetLinesOfCode(indent, signature, parameterNames, conditionsAndCodes))
        {
            if (parameterNames.Count() > signature.Types.Count() - 1)
                throw new ArgumentException("Too many parameter names");

            Signature = signature;
            ParameterNames = parameterNames;
            ConditionsAndCodes = conditionsAndCodes;
        }

        public PlutusFunctionSignature Signature { get; }

        public IEnumerable<string> ParameterNames { get; }

        public IEnumerable<(string, string)> ConditionsAndCodes { get; }


        static IEnumerable<IPlutusLine> GetLinesOfCode(int indent,
            PlutusFunctionSignature signature, 
            IEnumerable<string> parameterNames,
            IEnumerable<(string, string)> conditionsAndCodes)
        {
            var paramNames = parameterNames.Aggregate("", (acc, param) => acc + $" {param}");
            var firstLine = new PlutusRawLine(indent, $"{signature.Name}{paramNames}");

            var codeLines = new List<IPlutusLine>();
            foreach (var conditionAndCode in conditionsAndCodes)
            {
                codeLines.Add(
                    new PlutusRawLine(
                        indent + 1,
                        $"| {conditionAndCode.Item1} = {conditionAndCode.Item2}")
                );
            }

            return codeLines.Prepend(firstLine);
        }
    }
}
