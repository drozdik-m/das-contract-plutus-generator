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
                IsMandatory = source.IsMandatory
            };

            //Reference data type
            if (source.DataType == PropertyDataType.Reference ||
                source.DataType == PropertyDataType.Enum ||
                source.PropertyType == PropertyType.Dictionary)
                throw new Exception($"Data type is not primitive, but the convertor converts only primitives ({source.Id})");

            if (source.DataType is null)
                throw new Exception("Property data type was null");

            result.Type = typeConvertor.Convert(source.DataType.Value);

            //Cardinality
            if (source.PropertyType is null)
                throw new Exception("Property type was null (unknown cardinality)");

            result.Cardinality = cardinalityConvertor.Convert(source.PropertyType.Value);

            return result;
        }
    }
}
