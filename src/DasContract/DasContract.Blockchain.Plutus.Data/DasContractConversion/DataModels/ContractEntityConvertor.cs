using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Abstraction.Data;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;
using DasContract.Editor.Entities.DataModels.Entities.Properties.Reference;

namespace DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels
{
    public class ContractEntityConvertor : IConvertor<Entity, ContractEntity>
    {
        private readonly IConvertor<Property, PrimitiveContractProperty> primitivePropertyConvertor;
        private readonly IConvertor<Property, ReferenceContractProperty> referencePropertyConvertor;

        public ContractEntityConvertor(
            IConvertor<Property, PrimitiveContractProperty> primitivePropertyConvertor,
            IConvertor<Property, ReferenceContractProperty> referencePropertyConvertor)
        {
            this.primitivePropertyConvertor = primitivePropertyConvertor;
            this.referencePropertyConvertor = referencePropertyConvertor;
        }

        /// <inheritdoc/>
        public ContractEntity Convert(Entity source)
        {
            var result = new ContractEntity
            {
                Id = source.Id,
                Name = source.Name,
                IsRoot = source.IsRootEntity
            };

            //Convert properties
            foreach(var property in source.Properties)
            {
                
                //Primitive data type
                if (property.DataType != PropertyDataType.Reference && property.PropertyType != PropertyType.Dictionary)
                    result.AddProperty(primitivePropertyConvertor.Convert(property));

                //Reference data type
                else if (property.DataType == PropertyDataType.Reference)
                    result.AddProperty(referencePropertyConvertor.Convert(property));

                //TODO dictionary type
            }

            return result;
        }
    }
}
