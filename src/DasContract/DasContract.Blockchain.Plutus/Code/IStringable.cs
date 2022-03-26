using System;
using System.Collections.Generic;
using System.Text;

namespace DasContract.Blockchain.Plutus.Code
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
