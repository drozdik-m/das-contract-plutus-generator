using System;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process.Events
{
    public class ContractStartEvent : ContractEvent
    {
        public ContractProcessElement Outgoing { get; set; }
    }
}
