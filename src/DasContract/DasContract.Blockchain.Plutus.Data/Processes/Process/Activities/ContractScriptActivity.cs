using System;
using System.ComponentModel;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process.Activities
{
    public class ContractScriptActivity : ContractActivity
    {
        public string Script { get; set; } = string.Empty;
    }
}
