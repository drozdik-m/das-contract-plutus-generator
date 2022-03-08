using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Abstraction.Data;
using DasContract.Abstraction.Processes;
using DasContract.Abstraction.Processes.Events;
using DasContract.Abstraction.Processes.Tasks;
using DasContract.Blockchain.Plutus.Data.Abstraction;
using DasContract.Blockchain.Plutus.Data.DasContractConversion.Processes.Activities.MultiInstance;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;
using DasContract.Blockchain.Plutus.Data.Processes;
using DasContract.Blockchain.Plutus.Data.Processes.Process;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Activities;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Events;
using DasContract.Blockchain.Plutus.Data.Processes.Process.MultiInstances;

namespace DasContract.Blockchain.Plutus.Data.DasContractConversion.Processes
{
    public class ContractProcessesConvertor : IConvertor<IEnumerable<Process>, ContractProcesses>
    {
        private readonly IConvertor<Process, ContractProcess> processConvertor;

        public ContractProcessesConvertor(IConvertor<Process, ContractProcess> processConvertor)
        {
            this.processConvertor = processConvertor;
        }

        /// <inheritdoc/>
        public ContractProcesses Convert(IEnumerable<Process> source)
        {
            var processes = source
                .Select(e => processConvertor.Convert(e))
                .ToList();

            var callActivities = processes
                .Aggregate(
                    new List<ContractCallActivity>(),
                    (acc, item) =>
                    {
                        return acc
                            .Concat(item.ProcessElements.OfType<ContractCallActivity>())
                            .ToList();
                    });

            foreach(var activity in callActivities)
                ContractCallActivityConvertor.Bind(activity, processes);

            return new ContractProcesses
            {
                AllProcesses = processes
            };
        }

    }
}
