using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Abstraction.Data;
using DasContract.Blockchain.Plutus.Data.DataModels;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;

namespace DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels
{
    public class ContractDataModelConvertor : IConvertor<IEnumerable<Entity>, ContractDataModel>
    {
        private readonly IConvertor<Entity, ContractEntity> contractEntityConvertor;
        private readonly ReferencePropertyConvertor referencePropertyConvertor;

        public ContractDataModelConvertor(
            IConvertor<Entity, ContractEntity> contractEntityConvertor,
            ReferencePropertyConvertor referencePropertyConvertor)
        {
            this.contractEntityConvertor = contractEntityConvertor;
            this.referencePropertyConvertor = referencePropertyConvertor;
        }

        /// <inheritdoc/>
        public ContractDataModel Convert(IEnumerable<Entity> source)
        {
            var result = new ContractDataModel();
            
            foreach (var entity in source)
                result.AddEntity(contractEntityConvertor.Convert(entity));

            if (result.Entities.Where(e => e.IsRoot).Count() != 1)
                throw new Exception("Contract data model must contain exactly one root entity");

            foreach (var entity in result.Entities)
            {
                foreach (var referenceProperty in entity.ReferenceProperties)
                    referencePropertyConvertor.BindToEntity(referenceProperty, result.Entities);
            }

            return result;
        }
    }
}
