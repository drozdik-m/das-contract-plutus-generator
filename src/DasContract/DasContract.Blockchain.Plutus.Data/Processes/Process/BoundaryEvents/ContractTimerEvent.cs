using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Blockchain.Plutus.Data.Processes.Process;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process.Events
{
    public abstract class ContractTimerEvent : ContractProcessElement
    {
        public ContractProcessElement TimeOutDirection { get; set; }

        public string TimerDefinition { get; set; } = string.Empty;
    }
}
