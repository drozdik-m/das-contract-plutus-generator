using System.Collections.Generic;
using System.Linq;
using DasContract.Blockchain.Plutus.Data.Processes.Process;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Events;
using DasContract.Blockchain.Plutus.Data.Processes.Process.MultiInstances;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process.Activities
{
    public abstract class ContractActivity : ContractProcessElement
    {
        /// <summary>
        /// The next process element in the process
        /// </summary>
        public ContractProcessElement Outgoing { get; set; }

        /// <summary>
        /// Boundary events attached to this activity
        /// </summary>
        public ICollection<ContractBoundaryEvent> BoundaryEvents { get; set; } = new List<ContractBoundaryEvent>();

        /// <summary>
        /// Multiinstance type of this activity
        /// </summary>
        public ContractMultiInstance MultiInstance { get; set; } = ContractSingleMultiInstance.Instance;

        /// <inheritdoc/>
        public override void CollectSuccessors(ref Dictionary<string, ContractProcessElement> collector)
        {
            if (collector.ContainsKey(Id))
                return;

            base.CollectSuccessors(ref collector);
            Outgoing.CollectSuccessors(ref collector);
            foreach (var boundaryEvent in BoundaryEvents)
                boundaryEvent.CollectSuccessors(ref collector);
        }
    }
}
