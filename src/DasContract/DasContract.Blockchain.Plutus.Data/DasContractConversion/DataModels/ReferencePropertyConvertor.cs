using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Abstraction.Data;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;
using DasContract.Editor.Entities.DataModels.Entities.Properties.Reference;

namespace DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels
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
                Name = source.Name,
                IsMandatory = source.IsMandatory,
                EntityId = source.ReferencedDataType
            };

            //Reference data type
            if (source.DataType != PropertyDataType.Reference)
                throw new Exception("Data type not is reference, but the convertor converts only references");

            //Dictionary data type
            if (source.PropertyType == PropertyType.Dictionary)
                throw new Exception("Data type is dictionary, but the convertor converts only references");

            //Cardinality
            result.Cardinality = cardinalityConvertor.Convert(source.PropertyType);

            return result;
        }

        public ReferenceContractProperty BindToEntity (ReferenceContractProperty property, IEnumerable<ContractEntity> entities)
        {
            property.Entity = entities
                .Where(e => e.Id == property.EntityId)
                .Single();

            return property;
        }
    }
}
