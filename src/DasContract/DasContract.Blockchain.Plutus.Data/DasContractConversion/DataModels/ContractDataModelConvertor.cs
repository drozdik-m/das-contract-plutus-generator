using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Abstraction.Data;
using DasContract.Blockchain.Plutus.Data.Abstraction;
using DasContract.Blockchain.Plutus.Data.DataModels;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Reference;
using Enum = DasContract.Abstraction.Data.Enum;

namespace DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels
{
    public class ContractDataModelConvertor : IConvertor<(IEnumerable<Entity>, IEnumerable<Enum>), ContractDataModel>
    {
        private readonly IConvertor<Entity, ContractEntity> contractEntityConvertor;
        private readonly IConvertor<Property, ReferenceContractProperty> referencePropertyConvertor;
        private readonly IConvertor<Enum, ContractEnum> contractEnumConvertor;

        public ContractDataModelConvertor(
            IConvertor<Entity, ContractEntity> contractEntityConvertor,
            IConvertor<Property, ReferenceContractProperty> referencePropertyConvertor,
            IConvertor<Enum, ContractEnum> contractEnumConvertor
            )
        {
            this.contractEntityConvertor = contractEntityConvertor;
            this.referencePropertyConvertor = referencePropertyConvertor;
            this.contractEnumConvertor = contractEnumConvertor;
        }

        /// <inheritdoc/>
        public ContractDataModel Convert((IEnumerable<Entity>, IEnumerable<Enum>) source)
        {
            var result = new ContractDataModel();
            var sourceEntities = source.Item1;
            var sourceEnums = source.Item2;
            
            //Convert entities
            foreach (var entity in sourceEntities)
                result.AddEntity(contractEntityConvertor.Convert(entity));

            //Convert enums
            foreach (var sEnum in sourceEnums)
                result.AddEnum(contractEnumConvertor.Convert(sEnum));

            //Check there is only one root entity
            if (result.Entities.Where(e => e.IsRoot).Count() != 1)
                throw new Exception("Contract data model must contain exactly one root entity");

            //Bind references and links
            foreach (var entity in result.Entities)
            {
                foreach (var referenceProperty in entity.ReferenceProperties)
                    ReferencePropertyConvertor.Bind(referenceProperty, result.Entities);
                foreach (var dictionaryProperty in entity.DictionaryProperties)
                    DictionaryPropertyConvertor.Bind(dictionaryProperty, result.Entities, result.Enums);
                foreach (var enumProperty in entity.EnumProperties)
                    EnumPropertyConvertor.Bind(enumProperty, result.Enums);
            }

            return result;
        }
    }
}
