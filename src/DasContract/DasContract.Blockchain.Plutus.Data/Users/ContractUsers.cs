using System;
using System.Collections.Generic;
using System.Text;

namespace DasContract.Blockchain.Plutus.Data.Users
{
    /// <summary>
    /// Data model of contract information related to users and user identity
    /// </summary>
    public class ContractUsers
    {
        /// <summary>
        /// All contract users
        /// </summary>
        public ICollection<ContractUser> Users { get; set; } = new List<ContractUser>();

        /// <summary>
        /// All contract roles 
        /// </summary>
        public ICollection<ContractRole> Roles { get; set; } = new List<ContractRole>();
    }
}
