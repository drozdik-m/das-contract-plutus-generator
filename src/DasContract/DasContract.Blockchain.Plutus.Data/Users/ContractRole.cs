using System.Xml.Linq;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Data.Users
{
    /// <summary>
    /// A user role
    /// </summary>
    public class ContractRole : IIdentifiable, INamable
    {
        /// <inheritdoc/>
        public string Id { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string Name => Id;

        /// <summary>
        /// Description of this role
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
