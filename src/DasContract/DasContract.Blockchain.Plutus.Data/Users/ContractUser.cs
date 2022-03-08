using System.Collections.Generic;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Data.Users
{
    /// <summary>
    /// Representation of a user and his roles
    /// </summary>
    public class ContractUser : IIdentifiable, INamable
    {
        /// <inheritdoc/>
        public string Id { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string Name => Id;

        /// <summary>
        /// Description of this user
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Wallet address (= public key) of this user in hex format
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Roles of this user
        /// </summary>
        public ICollection<ContractRole> Roles { get; set; } = new List<ContractRole>();

        /// <summary>
        /// Role ids of this user
        /// </summary>
        public ICollection<string> RoleIds { get; set; } = new List<string>();
    }
}
