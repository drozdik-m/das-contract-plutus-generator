
using System;
using System.Collections.Generic;
using DasContract.Blockchain.Plutus.Data.Forms;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process.Activities
{
    public class ContractUserActivity : ContractActivity
    {
        public string ValidationScript { get; set; } = string.Empty;

        public ContractForm Form { get; set; } = new ContractForm();

        public string FormName => Name + "Form";
    }
}
