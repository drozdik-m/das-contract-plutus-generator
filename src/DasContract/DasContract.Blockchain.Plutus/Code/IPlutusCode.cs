using System;
using System.Collections.Generic;
using System.Text;

namespace DasContract.Blockchain.Plutus.Code
{
    public interface IPlutusCode : IStringable
    {
        /// <summary>
        /// Appends a code after this code
        /// </summary>
        /// <param name="code"></param>
        /// <returns>New plutus code</returns>
        IPlutusCode Append(IPlutusCode code);

        /// <summary>
        /// Appends a line after this code
        /// </summary>
        /// <param name="line"></param>
        /// <returns>New plutus code</returns>
        IPlutusCode Append(IPlutusLine line);

        /// <summary>
        /// Prepends a code before this code
        /// </summary>
        /// <param name="code"></param>
        /// <returns>New plutus code</returns>
        IPlutusCode Prepend(IPlutusCode code);

        /// <summary>
        /// Appends a line before this code
        /// </summary>
        /// <param name="line"></param>
        /// <returns>New plutus code</returns>
        IPlutusCode Prepend(IPlutusLine line);


    }

}
