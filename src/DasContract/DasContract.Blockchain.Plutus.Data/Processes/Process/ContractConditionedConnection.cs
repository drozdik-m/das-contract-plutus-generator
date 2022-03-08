using System;
using System.Collections.Generic;
using System.Text;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process
{
    /// <summary>
    /// A connection that has a condition. 
    /// Mainly for XOR gateways.
    /// </summary>
    public class ContractConditionedConnection
    {
        /// <summary>
        /// The condition of this connection
        /// </summary>
        public string Condition { get; set; } = string.Empty;

        /// <summary>
        /// The target of this connection
        /// </summary>
        public ContractProcessElement Target { get; set; }
    }
}
