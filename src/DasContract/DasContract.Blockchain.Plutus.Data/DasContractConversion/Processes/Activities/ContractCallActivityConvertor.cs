using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Abstraction.Processes;
using DasContract.Abstraction.Processes.Tasks;
using DasContract.Blockchain.Plutus.Data.Abstraction;
using DasContract.Blockchain.Plutus.Data.Processes.Process;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Activities;
using DasContract.Blockchain.Plutus.Data.Processes.Process.MultiInstances;
using DasContract.String.Utils;

namespace DasContract.Blockchain.Plutus.Data.DasContractConversion.Processes.Activities.MultiInstance
{
    public class ContractCallActivityConvertor : IConvertor<CallActivity, ContractCallActivity>
    {
        private readonly IConvertor<Task, ContractMultiInstance> multiInstanceConvertor;

        public ContractCallActivityConvertor(
            IConvertor<Task, ContractMultiInstance> multiInstanceConvertor)
        {
            this.multiInstanceConvertor = multiInstanceConvertor;
        }

        public ContractCallActivity Convert(CallActivity source)
        {
            var result = new ContractCallActivity()
            {
                Id = source.Id.FirstCharToUpperCase(),
                CalledProcessId = source.CalledElement,
            };

            result.MultiInstance = multiInstanceConvertor.Convert(source);

            return result;
        }

        public static ContractCallActivity Bind(ContractCallActivity callActivity,
            IEnumerable<ContractProcess> processes)
        {
            var suitableProcess = processes
                    .Where(e => e.Id == callActivity.CalledProcessId);

            if (suitableProcess.Count() != 1)
                throw new Exception($"Call activity {callActivity.Id} must have exactly one suitable calling subprocess");

            callActivity.CalledProcess = suitableProcess.Single();
            return callActivity;
        }
    }
}
