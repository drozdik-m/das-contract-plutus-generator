using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Blockchain.Plutus.Code;

namespace DasContract.Blockchain.Plutus
{
    public interface ILineable
    {
        /// <summary>
        /// Returns the content code-line form
        /// </summary>
        /// <returns></returns>
        IPlutusLine InLine();
    }

}
