using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DasContract.Abstraction.Processes;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Data.Users
{
    public class ContractUser : IIdentifiable, INamable
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public ICollection<ProcessRole> Roles { get; set; } = new List<ProcessRole>();
    }
}
