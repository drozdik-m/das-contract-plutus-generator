using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Abstraction.Data;
using DasContract.Blockchain.Plutus.Data.Abstraction;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;

namespace DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels
{
    public class EnumConvertor : IConvertor<DasContract.Abstraction.Data.Enum, ContractEnum>
    {

        /// <inheritdoc/>
        public ContractEnum Convert(DasContract.Abstraction.Data.Enum source)
        {
            if (source.Values.Count() == 0)
                throw new Exception($"Enum {source.Id} has zero values. Enums must have at least one value.");

            var result = new ContractEnum
            {
                Id = source.Id,
                Values = source.Values
            };

            return result;
        }
    }
}
