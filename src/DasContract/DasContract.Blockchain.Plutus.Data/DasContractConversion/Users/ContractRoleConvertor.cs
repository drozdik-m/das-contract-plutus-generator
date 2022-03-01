using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Abstraction.Data;
using DasContract.Abstraction.Processes;
using DasContract.Blockchain.Plutus.Data.Abstraction;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;
using DasContract.Blockchain.Plutus.Data.Users;

namespace DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels
{
    public class ContractRoleConvertor : IConvertor<ProcessRole, ContractRole>
    {

        /// <inheritdoc/>
        public ContractRole Convert(ProcessRole source)
        {
            var result = new ContractRole()
            {
                Id = source.Name,
                Description = source.Description,
            };

            return result;
        }
    }
}
