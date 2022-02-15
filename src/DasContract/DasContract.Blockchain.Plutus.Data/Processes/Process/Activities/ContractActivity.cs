using System.Collections.Generic;
using DasContract.Blockchain.Plutus.Data.Processes.Process;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Events;
using DasContract.Blockchain.Plutus.Data.Processes.Process.MultiInstances;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process.Activities
{
    public abstract class ContractActivity : ContractProcessElement
    {
        public ContractProcessElement Incoming { get; set; }

        public ContractProcessElement Outgoing { get; set; }

        public ICollection<ContractEvent> Events { get; set; } = new List<ContractEvent>(); 

        public ContractMultiInstance? MultiInstance { get; set; }
    }
}
