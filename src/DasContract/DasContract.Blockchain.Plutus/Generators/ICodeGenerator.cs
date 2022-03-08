using System;
using System.Collections.Generic;
using System.Text;

namespace DasContract.Blockchain.Plutus.Generators
{
    public interface ICodeGenerator
    {
        /// <summary>
        /// Generates a code
        /// </summary>
        /// <returns></returns>
        public IPlutusCode Generate();
    }
}
