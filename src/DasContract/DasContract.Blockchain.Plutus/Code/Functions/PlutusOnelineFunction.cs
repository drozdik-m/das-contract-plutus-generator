using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DasContract.Blockchain.Plutus.Code.Comments
{
    /// <summary>
    /// Shorter version of the plutus function that is just a one-liner
    /// </summary>
    public class PlutusOnelineFunction : PlutusLine
    {
        public PlutusOnelineFunction(int indent, 
            PlutusFunctionSignature signature, 
            IEnumerable<string> parameterNames,
            string code): base(indent)
        {
            Signature = signature;
            ParameterNames = parameterNames;
            Code = code;

            if (parameterNames.Count() > signature.Types.Count() - 1)
                throw new ArgumentException("Too many parameter names");
        }

        public PlutusFunctionSignature Signature { get; }

        public IEnumerable<string> ParameterNames { get; }

        public string Code { get; }


        /// <inheritdoc/>
        public override string InString()
        {
            var paramNames = ParameterNames
                .Aggregate("", (acc, item) => acc + item + " ");
            return base.InString() + $"{Signature.Name} {paramNames}= {Code}";
        }
    }
}
