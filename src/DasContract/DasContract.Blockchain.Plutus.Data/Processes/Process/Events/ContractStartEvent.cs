using System;
using System.Collections.Generic;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process.Events
{
    public class ContractStartEvent : ContractEvent
    {
        /// <summary>
        /// The outgoing connection of this event
        /// </summary>
        public ContractProcessElement Outgoing { get; set; }

        /// <inheritdoc/>
        public override void CollectSuccessors(ref Dictionary<string, ContractProcessElement> collector)
        {
            if (collector.ContainsKey(Id))
                return;

            base.CollectSuccessors(ref collector);
            Outgoing.CollectSuccessors(ref collector);
        }

        /// <inheritdoc/>
        public override T Accept<T>(IContractProcessElementVisitor<T> visitor)
            => visitor.Visit(this);
    }
}
