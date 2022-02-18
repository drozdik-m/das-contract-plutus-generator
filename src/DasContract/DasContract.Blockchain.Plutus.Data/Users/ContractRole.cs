using System.Xml.Linq;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Data.Users
{
    public class ContractRole : IIdentifiable
    {
        public string Id { get; set; } = string.Empty;

        public string Name => Id;

        public string Description { get; set; } = string.Empty;
    }
}
