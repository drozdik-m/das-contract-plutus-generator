using System;
using System.ComponentModel;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process.Activities
{
    public class ContractCallActivity : ContractActivity
    {
        public ContractProcess CalledProcess { get; set; }

        public string CalledProcessId { get; set; } = string.Empty;

        /// <inheritdoc/>
        public override T Accept<T>(IContractProcessElementVisitor<T> visitor)
            => visitor.Visit(this);
    }
}
