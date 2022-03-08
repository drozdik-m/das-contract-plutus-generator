using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Blockchain.Plutus.Code;

namespace DasContract.Blockchain.Plutus
{
    public interface IStringable
    {
        /// <summary>
        /// Returns the content string form
        /// </summary>
        /// <returns></returns>
        string InString();
    }

}
