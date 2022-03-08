using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Abstraction.Processes;
using DasContract.Abstraction.Processes.Gateways;
using DasContract.Abstraction.Processes.Tasks;
using DasContract.Blockchain.Plutus.Data.Abstraction;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Activities;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Gateways;
using DasContract.Blockchain.Plutus.Data.Processes.Process.MultiInstances;

namespace DasContract.Blockchain.Plutus.Data.DasContractConversion.Processes.Activities.MultiInstance
{
    public class ContractMergingExclusiveGatewayConvertor : IConvertor<ExclusiveGateway, ContractMergingExclusiveGateway>
    {

        public ContractMergingExclusiveGateway Convert(ExclusiveGateway source)
        {
            if (source.Outgoing.Count() != 1)
                throw new Exception($"Merging exclusive gateway {source.Id} needs exactly one output");

            var result = new ContractMergingExclusiveGateway()
            {
                Id = source.Id,
            };

            return result;
        }
    }
}
