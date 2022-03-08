using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Abstraction.Data;
using DasContract.Blockchain.Plutus.Data.Abstraction;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Reference;

namespace DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels.Properties.Reference
{
    public class ReferencePropertyConvertor : IConvertor<Property, ReferenceContractProperty>
    {
        private readonly IConvertor<PropertyType, ContractPropertyCardinality> cardinalityConvertor;

        public ReferencePropertyConvertor(IConvertor<PropertyType, ContractPropertyCardinality> cardinalityConvertor)
        {
            this.cardinalityConvertor = cardinalityConvertor;
        }

        /// <inheritdoc/>
        public ReferenceContractProperty Convert(Property source)
        {
            var result = new ReferenceContractProperty
            {
                Id = source.Id,
                IsMandatory = source.IsMandatory,
                EntityId = source.ReferencedDataType
            };

            //Reference data type
            if (source.DataType != PropertyDataType.Reference || source.PropertyType == PropertyType.Dictionary)
                throw new Exception($"Data type {source.Id} is not reference, but the convertor converts only references");

            //Cardinality
            if (source.PropertyType is null)
                throw new Exception($"Data type {source.Id} has undefined property type");
            result.Cardinality = cardinalityConvertor.Convert(source.PropertyType.Value);

            return result;
        }

        public static ReferenceContractProperty Bind(ReferenceContractProperty property, IEnumerable<ContractEntity> entities)
        {
            var suitableEntities = entities
                .Where(e => e.Id == property.EntityId);

            if (suitableEntities.Count() != 1)
                throw new Exception($"Too much or none entities with id {property.EntityId} found");

            property.Entity = suitableEntities.Single();
            return property;
        }
    }
}
