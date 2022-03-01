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
    public class ContractUsersConvertor : IConvertor<(IEnumerable<ProcessUser>, IEnumerable<ProcessRole>), ContractUsers>
    {
        private readonly IConvertor<ProcessRole, ContractRole> roleConvertor;
        private readonly IConvertor<ProcessUser, ContractUser> userConvertor;

        public ContractUsersConvertor(
            IConvertor<ProcessRole, ContractRole> roleConvertor,
            IConvertor<ProcessUser, ContractUser> userConvertor)
        {
            this.roleConvertor = roleConvertor;
            this.userConvertor = userConvertor;
        }


        /// <inheritdoc/>
        public ContractUsers Convert((IEnumerable<ProcessUser>, IEnumerable<ProcessRole>) source)
        {
            var sourceUsers = source.Item1;
            var sourceRoles = source.Item2;

            var roles = sourceRoles
                .Select(e => roleConvertor.Convert(e))
                .ToList();
            var users = sourceUsers
                .Select(e => userConvertor.Convert(e))
                .ToList();

            foreach (var user in users)
                ContractUserConvertor.Bind(user, roles);

            return new ContractUsers()
            {
                Roles = roles,
                Users = users
            };
        }
    }
}
