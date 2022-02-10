using System;
using System.Collections.Generic;
using System.Text;

namespace DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels
{
    public interface IConvertor<TSource, TTarget>
    {
        /// <summary>
        /// Converts from source to a target
        /// </summary>
        /// <param name="source">The source to convert</param>
        /// <returns>The converted source to target</returns>
        public TTarget Convert(TSource source);
    }
}
