using System.Collections.Generic;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Data.Users
{
    public class ContractUser : IIdentifiable, INamable
    {
        public string Id { get; set; } = string.Empty;

        public string Name => Id;

        public string Description { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public ICollection<ContractRole> Roles { get; set; } = new List<ContractRole>();
    }
}
