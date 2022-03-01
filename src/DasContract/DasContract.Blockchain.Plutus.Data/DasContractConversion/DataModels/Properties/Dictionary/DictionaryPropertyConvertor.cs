using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Abstraction.Data;
using DasContract.Blockchain.Plutus.Data.Abstraction;
using DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels.Properties.Enum;
using DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels.Properties.Reference;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Dictionary;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Enum;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Reference;

namespace DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels.Properties.Dictionary
{
    public class DictionaryPropertyConvertor : IConvertor<Property, DictionaryContractProperty>
    {
        private readonly IConvertor<PropertyDataType, PrimitiveContractPropertyType> primTypeConvertor;

        public DictionaryPropertyConvertor(
            IConvertor<PropertyDataType, PrimitiveContractPropertyType> primTypeConvertor)
        {
            this.primTypeConvertor = primTypeConvertor;
        }

        /// <inheritdoc/>
        public DictionaryContractProperty Convert(Property source)
        {
            var result = new DictionaryContractProperty
            {
                Id = source.Id,
                IsMandatory = source.IsMandatory,
            };

            //Non-dictionary data type
            if (source.PropertyType != PropertyType.Dictionary)
                throw new Exception($"Data type {source.Id} is not dictionary, but the convertor converts only dictionaries");

            //Key type
            if (source.KeyType is null)
                throw new Exception($"Key type is not defined for a dictionary {source.Id}");
            var keyType = primTypeConvertor.Convert(source.KeyType.Value);
            result.KeyType = keyType;

            //Value type
            ContractProperty valueProperty;
            if (source.DataType is null)
                throw new Exception($"Data type is not defined for a dictionary {source.Id}");
            if (source.DataType == PropertyDataType.Reference)
            {
                valueProperty = new ReferenceContractProperty()
                {
                    Id = source.Id + "Key",
                    Cardinality = ContractPropertyCardinality.Single,
                    IsMandatory = true,
                    EntityId = source.ReferencedDataType,
                };
            }
            else if (source.DataType == PropertyDataType.Enum)
            {
                valueProperty = new EnumContractProperty()
                {
                    Id = source.Id + "Key",
                    Cardinality = ContractPropertyCardinality.Single,
                    IsMandatory = true,
                    EnumEntityId = source.ReferencedDataType,
                };
            }
            else
            {
                valueProperty = new PrimitiveContractProperty()
                {
                    Id = source.Id + "Key",
                    Cardinality = ContractPropertyCardinality.Single,
                    IsMandatory = true,
                    Type = primTypeConvertor.Convert(source.DataType.Value),
                };
            }

            result.ValueType = valueProperty;
            return result;
        }

        public static DictionaryContractProperty Bind(DictionaryContractProperty property,
            IEnumerable<ContractEntity> entities,
            IEnumerable<ContractEnum> enums)
        {
            if (property.ValueType is ReferenceContractProperty refProp)
                ReferencePropertyConvertor.Bind(refProp, entities);
            else if (property.ValueType is EnumContractProperty enumProp)
                EnumPropertyConvertor.Bind(enumProp, enums);
            return property;
        }
    }
}
