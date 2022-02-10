using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Abstraction.Data;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;

namespace DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels
{
    public class ContractEntityConvertor : IConvertor<Entity, ContractEntity>
    {
        private readonly PrimitivePropertyConvertor primitivePropertyConvertor;
        private readonly ReferencePropertyConvertor referencePropertyConvertor;

        public ContractEntityConvertor(
            PrimitivePropertyConvertor primitivePropertyConvertor,
            ReferencePropertyConvertor referencePropertyConvertor)
        {
            this.primitivePropertyConvertor = primitivePropertyConvertor;
            this.referencePropertyConvertor = referencePropertyConvertor;
        }

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
