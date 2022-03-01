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
    public class ContractPropertyConvertor : IConvertor<Property, ContractProperty>
    {
        private readonly IConvertor<Property, PrimitiveContractProperty> primitivePropertyConvertor;
        private readonly IConvertor<Property, ReferenceContractProperty> referencePropertyConvertor;
        private readonly IConvertor<Property, DictionaryContractProperty> dictionaryPropertyConvertor;
        private readonly IConvertor<Property, EnumContractProperty> enumPropertyConvertor;

        public ContractPropertyConvertor(
            IConvertor<Property, PrimitiveContractProperty> primitivePropertyConvertor,
            IConvertor<Property, ReferenceContractProperty> referencePropertyConvertor,
            IConvertor<Property, DictionaryContractProperty> dictionaryPropertyConvertor,
            IConvertor<Property, EnumContractProperty> enumPropertyConvertor)
        {
            this.primitivePropertyConvertor = primitivePropertyConvertor;
            this.referencePropertyConvertor = referencePropertyConvertor;
            this.dictionaryPropertyConvertor = dictionaryPropertyConvertor;
            this.enumPropertyConvertor = enumPropertyConvertor;
        }

        /// <inheritdoc/>
        public ContractProperty Convert(Property source)
        {
            //Dictionary type
            if (source.PropertyType == PropertyType.Dictionary)
                return dictionaryPropertyConvertor.Convert(source);

            //Enum type
            else if (source.DataType == PropertyDataType.Enum)
                return enumPropertyConvertor.Convert(source);

            //Reference data type
            else if (source.DataType == PropertyDataType.Reference)
                return referencePropertyConvertor.Convert(source);

            //Primitive data type
            else 
                return primitivePropertyConvertor.Convert(source);

            //throw new Exception("Unhandled type of property");
        }
    }
}
