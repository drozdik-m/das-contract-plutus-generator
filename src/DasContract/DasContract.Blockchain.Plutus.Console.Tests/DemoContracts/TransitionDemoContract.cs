using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DasContract.Blockchain.Plutus.Data;
using DasContract.Blockchain.Plutus.Data.Processes;
using DasContract.Blockchain.Plutus.Data.Processes.Process;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Activities;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Events;

namespace DasContract.Blockchain.Plutus.Console.Tests.DemoContracts
{
    public static class TransitionDemoContract
    {
        public static PlutusContract Get()
        {
            //--- MAIN PROCESS ---
            var mainStart = new ContractStartEvent() { Id = "StartEvent" };
            var mainEnd = new ContractStartEvent() { Id = "EndEvent" };

            var script1 = new ContractScriptActivity()
            {
                Id = "Script1",
                Code = ContractScriptActivity.TransitionPragma + Environment.NewLine +
                       "ahoj to je kod" + Environment.NewLine +
                       "\t ano vskutku" + Environment.NewLine +
                       "yep" + Environment.NewLine,
            };

            var mainProcess = new ContractProcess()
            {
                IsMain = true,
                StartEvent = mainStart,
            };

            //--- CONTRACT ---
            var contract = new PlutusContract()
            {
                 Processes = new ContractProcesses()
                 {
                       AllProcesses = new ContractProcess[]
                       {
                           mainProcess,
                       }
                 }
            };
            
            return contract;    
        }
    }
}
