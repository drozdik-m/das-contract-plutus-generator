using DasContract.Blockchain.Plutus.Data.Processes.Process.Events;
using DasContract.Editor.Entities.Processes.Process.Activities;
using DasContract.Editor.Entities.Processes.Process.Gateways;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process
{
    public class ContractProcess
    {
        public ContractStartEvent StartEvent { get; set; }

        public bool IsMain { get; set; } = false;

        public static ContractProcess Empty()
        {
            return new ContractProcess();
        }
    }
}
