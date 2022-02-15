using System.Collections.Generic;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process.Gateways
{
    public class ContractMergingExclusiveGateway : ContractGateway
    {
        public ICollection<ContractProcessElement> Incoming { get; set; } = new List<ContractProcessElement>();

        public ContractProcessElement Outgoing { get; set; }
    }
}
