using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Abstraction.Processes;
using DasContract.Abstraction.Processes.Tasks;
using DasContract.Blockchain.Plutus.Data.Abstraction;
using DasContract.Blockchain.Plutus.Data.Processes.Process.MultiInstances;

namespace DasContract.Blockchain.Plutus.Data.DasContractConversion.Processes.Activities.MultiInstance
{
    public class ContractSequentialMultiInstanceConvertor : IConvertor<Task, ContractMultiInstance>
    {
        public ContractMultiInstance Convert(Task source)
        {
            if (source.InstanceType == InstanceType.Single)
                return ContractSingleMultiInstance.Instance;

            else if (source.InstanceType == InstanceType.Sequential)
            {
                return new ContractSequentialMultiInstance()
                {
                    LoopCardinality = source.LoopCardinality.ToString(),
                };
            }

            else if (source.InstanceType == InstanceType.Parallel)
                throw new Exception($"Parallel multiinstances are not supported ({source.Id})");

            throw new Exception("Unhandled type of InstanceType: " + source.InstanceType.GetTypeCode().ToString());
        }
    }
}
