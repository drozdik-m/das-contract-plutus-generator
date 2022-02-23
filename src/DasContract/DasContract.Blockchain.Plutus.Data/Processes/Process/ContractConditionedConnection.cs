using System;
using System.Collections.Generic;
using System.Text;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process
{
    public class ContractConditionedConnection
    {
        public string Condition { get; set; } = string.Empty;

        public ContractProcessElement Target { get; set; }
    }
}
