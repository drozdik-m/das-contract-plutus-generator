using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Abstraction.Data;
using DasContract.Blockchain.Plutus.Data.Abstraction;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Enum;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Reference;

namespace DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels
{
    public class EnumPropertyConvertor : IConvertor<Property, EnumContractProperty>
    {
        private readonly IConvertor<PropertyType, ContractPropertyCardinality> cardinalityConvertor;

        public EnumPropertyConvertor(IConvertor<PropertyType, ContractPropertyCardinality> cardinalityConvertor)
        {
            this.cardinalityConvertor = cardinalityConvertor;
        }

        /// <inheritdoc/>
        public EnumContractProperty Convert(Property source)
        {
            var result = new EnumContractProperty
            {
                Id = source.Id,
                IsMandatory = source.IsMandatory,
                EnumEntityId = source.ReferencedDataType,
            };

            //Reference data type
            if (source.DataType != PropertyDataType.Enum || source.PropertyType == PropertyType.Dictionary)
                throw new Exception($"Data type {source.Id} is not enum, but the convertor converts only enums");

            //Cardinality
            if (source.PropertyType is null)
                throw new Exception($"Data type {source.Id} has undefined property type");
            result.Cardinality = cardinalityConvertor.Convert(source.PropertyType.Value);

            return result;
        }

        public static EnumContractProperty Bind (EnumContractProperty property, IEnumerable<ContractEnum> enums)
        {
            var suitableEntities = enums
                .Where(e => e.Id == property.EnumEntityId);

            if (suitableEntities.Count() != 1)
                throw new Exception($"Too much or none enums with id {property.EnumEntityId} found");

            property.EnumEntity = suitableEntities.Single();
            return property;
        }
    }
}
