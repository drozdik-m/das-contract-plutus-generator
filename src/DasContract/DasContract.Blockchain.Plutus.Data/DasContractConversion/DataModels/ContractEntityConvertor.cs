using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Abstraction.Data;
using DasContract.Blockchain.Plutus.Data.Abstraction;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Dictionary;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Enum;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Reference;

namespace DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels
{
    public class ContractEntityConvertor : IConvertor<Entity, ContractEntity>
    {
        private readonly IConvertor<Property, ContractProperty> propertyConvertor;

        public ContractEntityConvertor(IConvertor<Property, ContractProperty> propertyConvertor)
        {
            this.propertyConvertor = propertyConvertor;
        }

        /// <inheritdoc/>
        public ContractEntity Convert(Entity source)
        {
            var result = new ContractEntity
            {
                Id = source.Id,
                IsRoot = source.IsRootEntity
            };

            //Convert properties
            foreach(var property in source.Properties)
            {
                var convProperty = propertyConvertor.Convert(property);

                //Primitive data type
                if (convProperty is PrimitiveContractProperty pProp)
                    result.AddProperty(pProp);

                //Reference data type
                else if (convProperty is ReferenceContractProperty rProp)
                    result.AddProperty(rProp);

                //Dictionary type
                else if (convProperty is DictionaryContractProperty dProp)
                    result.AddProperty(dProp);

                //Enum type
                else if (convProperty is EnumContractProperty eProp)
                    result.AddProperty(eProp);

                else
                    throw new Exception("Unhandled property type");
            }

            return result;
        }
    }
}
