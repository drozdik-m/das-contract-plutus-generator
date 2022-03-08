using System;
using System.Collections.Generic;
using System.Text;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process.MultiInstances
{
    public class ContractSequentialMultiInstance : ContractMultiInstance
    {
        /// <summary>
        /// How many iterations should occur
        /// </summary>
        public string LoopCardinality { get; set; } = string.Empty;
    }
}
