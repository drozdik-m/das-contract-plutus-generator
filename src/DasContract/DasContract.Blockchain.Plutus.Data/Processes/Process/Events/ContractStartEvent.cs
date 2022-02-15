using System;
using System.Collections.Generic;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process.Events
{
    public class ContractStartEvent : ContractEvent
    {
        public ContractProcessElement Outgoing { get; set; }

        public override void CollectSuccessors(ref Dictionary<string, ContractProcessElement> collector)
        {
            base.CollectSuccessors(ref collector);
            Outgoing.CollectSuccessors(ref collector);
        }
    }
}
