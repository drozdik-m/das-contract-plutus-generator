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
    public class ContractExclusiveGatewayConvertor : IConvertor<ExclusiveGateway, ContractExclusiveGateway>
    {

        public ContractExclusiveGateway Convert(ExclusiveGateway source)
        {
            if (source.Outgoing.Count() < 1)
                throw new Exception($"Exclusive gateway {source.Id} needs at lease one output");

            var result = new ContractExclusiveGateway()
            {
                Id = source.Id,
            };

            return result;
        }
    }
}
