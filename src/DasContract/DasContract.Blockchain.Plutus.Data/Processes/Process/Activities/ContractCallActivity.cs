using System;
using System.ComponentModel;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process.Activities
{
    public class ContractCallActivity : ContractActivity
    {
        public ContractProcess CalledProcess { get; set; }
    }
}
