using System.Collections.Generic;
using System.Linq;
using DasContract.Blockchain.Plutus.Data.Processes.Process;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Events;
using DasContract.Blockchain.Plutus.Data.Processes.Process.MultiInstances;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process.Activities
{
    public abstract class ContractActivity : ContractProcessElement
    {
        //public ContractProcessElement Incoming { get; set; }

        public ContractProcessElement Outgoing { get; set; }

        public ICollection<ContractBoundaryEvent> BoundaryEvents { get; set; } = new List<ContractBoundaryEvent>();

        public ContractMultiInstance MultiInstance { get; set; } = ContractSingleMultiInstance.Instance;

        public override void CollectSuccessors(ref Dictionary<string, ContractProcessElement> collector)
        {
            base.CollectSuccessors(ref collector);
            Outgoing.CollectSuccessors(ref collector);
            foreach (var boundaryEvent in BoundaryEvents)
                boundaryEvent.CollectSuccessors(ref collector);
        }
    }
}
