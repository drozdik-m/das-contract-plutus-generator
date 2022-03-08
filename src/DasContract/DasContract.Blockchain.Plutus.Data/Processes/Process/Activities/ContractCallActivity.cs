using System;
using System.ComponentModel;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process.Activities
{
    public class ContractCallActivity : ContractActivity
    {
        /// <summary>
        /// Process that is called by this activity
        /// </summary>
        public ContractProcess? CalledProcess { get; set; }

        /// <summary>
        /// Process id that is called by this activity
        /// </summary>
        public string CalledProcessId { get; set; } = string.Empty;

        /// <inheritdoc/>
        public override T Accept<T>(IContractProcessElementVisitor<T> visitor)
            => visitor.Visit(this);
    }
}
