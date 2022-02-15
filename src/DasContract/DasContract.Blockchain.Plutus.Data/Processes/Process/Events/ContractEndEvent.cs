using System;
using System.Collections.Generic;
using System.Text;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process.Events
{
    public class ContractEndEvent : ContractEvent
    {
        public ContractProcessElement Incoming { get; set; }
    }
}
