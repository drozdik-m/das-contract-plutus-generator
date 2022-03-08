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
    public class ContractUserConvertor : IConvertor<ProcessUser, ContractUser>
    {

        /// <inheritdoc/>
        public ContractUser Convert(ProcessUser source)
        {
            var result = new ContractUser()
            {
                Id = source.Name,
                Description = source.Description,
                Address = source.Address,
                RoleIds = source.Roles.Select(e => e.Name).ToList(),
            };

            return result;
        }

        public static ContractUser Bind(ContractUser user, IEnumerable<ContractRole> roles)
        {
            var suitableRoles = roles
                .Where(e => user.RoleIds.Contains(e.Id))
                .ToList();
            user.Roles = suitableRoles;
            return user;
        }
    }
}
