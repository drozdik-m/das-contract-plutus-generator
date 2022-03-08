using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Abstraction.Data;
using DasContract.Blockchain.Plutus.Data.Abstraction;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;

namespace DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels.Properties
{
    public class PropertyCardinalityConvertor : IConvertor<PropertyType, ContractPropertyCardinality>
    {
        /// <inheritdoc/>
        public ContractPropertyCardinality Convert(PropertyType source)
        {
            return source switch
            {
                PropertyType.Single => ContractPropertyCardinality.Single,
                PropertyType.Collection => ContractPropertyCardinality.Collection,
                _ => throw new Exception("No suitable cardinality found"),
            };
        }
    }
}
