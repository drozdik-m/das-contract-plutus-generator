using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Blockchain.Plutus.Data.Processes.Process;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process.Events
{
    public class ContractTimerBoundaryEvent : ContractBoundaryEvent
    {
        public ContractProcessElement TimeOutDirection { get; set; }

        public string TimerDefinitionExpr { get; set; } = string.Empty;

        public override void CollectSuccessors(ref Dictionary<string, ContractProcessElement> collector)
        {
            base.CollectSuccessors(ref collector);
            TimeOutDirection.CollectSuccessors(ref collector);
        }

        /// <inheritdoc/>
        public override T Accept<T>(IContractProcessElementVisitor<T> visitor)
            => visitor.Visit(this);
    }
}
