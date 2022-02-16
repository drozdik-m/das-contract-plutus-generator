using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Abstraction.Data;
using DasContract.Blockchain.Plutus.Data.Abstraction;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;

namespace DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels
{
    public class PrimitivePropertyConvertor : IConvertor<Property, PrimitiveContractProperty>
    {
        private readonly IConvertor<PropertyDataType, PrimitiveContractPropertyType> typeConvertor;
        private readonly IConvertor<PropertyType, ContractPropertyCardinality> cardinalityConvertor;

        public PrimitivePropertyConvertor(IConvertor<PropertyDataType, PrimitiveContractPropertyType> typeConvertor,
            IConvertor<PropertyType, ContractPropertyCardinality> cardinalityConvertor)
        {
            this.typeConvertor = typeConvertor;
            this.cardinalityConvertor = cardinalityConvertor;
        }

        /// <inheritdoc/>
        public PrimitiveContractProperty Convert(Property source)
        {
            var result = new PrimitiveContractProperty
            {
                Id = source.Id,
                Name = source.Name,
                IsMandatory = source.IsMandatory
            };

            //Reference data type
            if (source.DataType == PropertyDataType.Reference)
                throw new Exception("Data type is reference, but the convertor converts only primitives");

            //Dictionary data type
            if (source.PropertyType == PropertyType.Dictionary)
                throw new Exception("Data type is dictionary, but the convertor converts only primitives");

            result.Type = typeConvertor.Convert(source.DataType);

            //Cardinality
            result.Cardinality = cardinalityConvertor.Convert(source.PropertyType);

            return result;
        }
    }
}
