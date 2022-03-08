using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;

namespace DasContract.Blockchain.Plutus.Generators.Snippets
{
    public class PlutusContractModuleGenerator : ICodeGenerator
    {
        public IPlutusCode Generate()
        {
            var module = new PlutusCode(new List<IPlutusLine>()
            {
                new PlutusRawLine(0, "module PlutusContract"),
                    new PlutusRawLine(1, "(module PlutusContract)"),
                    new PlutusRawLine(1, "where"),

                PlutusLine.Empty,
                PlutusLine.Empty,
            });

            return module;
        }
    }
}
