using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Abstraction;
using DasContract.Abstraction.Data;
using DasContract.Blockchain.Plutus.Data.DataModels;

namespace DasContract.Blockchain.Plutus.Data
{
    public class DasContractToPlutusContractConvertor
    {
        public PlutusContract ToPlutusContract(Contract dasContract)
        {
            //TODO the rest

            throw new NotImplementedException();
        }

        public ContractDataModel ToPlutusDataModel(IEnumerable<Entity> dasContractEntities)
        {
            var dataModel = new ContractDataModel();
            throw new NotImplementedException();
        }

        public ContractDataModel ToPlutusEntity(IEnumerable<Entity> dasContractEntities)
        {
            throw new NotImplementedException();
        }

        public ContractDataModel ToPlutusProperty(IEnumerable<Entity> dasContractEntities)
        {
            throw new NotImplementedException();
        }
    }
}
