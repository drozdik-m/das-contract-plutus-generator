using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Blockchain.Plutus.Code;

namespace DasContract.Blockchain.Plutus
{
    public interface ICodeable
    {
        /// <summary>
        /// Returns the content code form
        /// </summary>
        /// <returns></returns>
        IPlutusCode InCode();
    }

}
