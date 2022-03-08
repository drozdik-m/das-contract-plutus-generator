using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Abstraction.Processes;
using DasContract.Abstraction.Processes.Tasks;
using DasContract.Blockchain.Plutus.Data.Abstraction;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Activities;
using DasContract.Blockchain.Plutus.Data.Processes.Process.MultiInstances;

namespace DasContract.Blockchain.Plutus.Data.DasContractConversion.Processes.Activities.MultiInstance
{
    public class ContractScriptActivityConvertor : IConvertor<ScriptTask, ContractScriptActivity>
    {
        private readonly IConvertor<Task, ContractMultiInstance> multiInstanceConvertor;

        public ContractScriptActivityConvertor(
            IConvertor<Task, ContractMultiInstance> multiInstanceConvertor)
        {
            this.multiInstanceConvertor = multiInstanceConvertor;
        }

        public ContractScriptActivity Convert(ScriptTask source)
        {
            var result = new ContractScriptActivity()
            {
                Id = source.Id,
                Code = source.Script
            };

            result.MultiInstance = multiInstanceConvertor.Convert(source);

            return result;
        }
    }
}
