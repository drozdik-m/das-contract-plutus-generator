using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Blockchain.Plutus.Data.Processes.Process;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process.Events
{
    public class ContractTimerBoundaryEvent : ContractBoundaryEvent
    {
        /// <summary>
        /// Direction of the process if this event times out
        /// </summary>
        public ContractProcessElement TimeOutDirection { get; set; }

        /// <summary>
        /// When the timer should time out
        /// </summary>
        public string TimerDefinition { get; set; } = string.Empty;

        /// <inheritdoc/>
        public override void CollectSuccessors(ref Dictionary<string, ContractProcessElement> collector)
        {
            if (collector.ContainsKey(Id))
                return;

            base.CollectSuccessors(ref collector);
            TimeOutDirection.CollectSuccessors(ref collector);
        }

        /// <inheritdoc/>
        public override T Accept<T>(IContractProcessElementVisitor<T> visitor)
            => visitor.Visit(this);
    }
}
