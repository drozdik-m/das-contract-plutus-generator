using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Abstraction;
using DasContract.Abstraction.Data;
using DasContract.Blockchain.Plutus.Data.DataModels;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;

namespace DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels
{
    public class PlutusContractConvertor : IConvertor<Contract, PlutusContract>
    {
        private readonly ContractDataModelConvertor contractDataModelConvertor;

        public PlutusContractConvertor(ContractDataModelConvertor contractDataModelConvertor)
        {
            this.contractDataModelConvertor = contractDataModelConvertor;
        }

        public PlutusContract Convert(Contract source)
        {
            var result = new PlutusContract
            {
                DataModel = contractDataModelConvertor.Convert(source.Entities)
            };

            return result;
        }
    }
}
