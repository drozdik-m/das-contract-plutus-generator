using System;
using System.Collections.Generic;
using System.Text;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process.MultiInstances
{
    public class ContractSingleMultiInstance : ContractMultiInstance
    {
        /// <summary>
        /// The singleton instance of ContractSingleMultiInstance
        /// </summary>
        public static ContractSingleMultiInstance Instance { get; } = new ContractSingleMultiInstance();
    }
}
