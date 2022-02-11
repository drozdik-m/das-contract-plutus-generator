using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Blockchain.Plutus.Code;

namespace DasContract.Blockchain.Plutus
{
    public interface IPlutusCode : IStringable
    {
        /// <summary>
        /// Appends a code after this code
        /// </summary>
        /// <param name="code"></param>
        /// <returns>New plutus code</returns>
        public IPlutusCode Append(IPlutusCode code);

        /// <summary>
        /// Prepends a code before this code
        /// </summary>
        /// <param name="code"></param>
        /// <returns>New plutus code</returns>
        public IPlutusCode Prepend(IPlutusCode code);
    }

}
