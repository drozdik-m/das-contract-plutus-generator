using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Abstraction.Data;
using DasContract.Abstraction.Processes;
using DasContract.Abstraction.Processes.Events;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;
using DasContract.Blockchain.Plutus.Data.Processes.Process;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Events;

namespace DasContract.Blockchain.Plutus.Data.DasContractConversion.Processes
{
    public class PrimitivePropertyTypeConvertor : IConvertor<Process, ContractProcess>
    {
        public ContractProcess Convert(Process source)
        {
            var result = new ContractProcess
            {
                IsMain = source.IsExecutable
            };

            //Start element
            var start = source.ProcessElements.Values.OfType<StartEvent>().SingleOrDefault();
            if (start is null)
                throw new ArgumentException("Process needs to have exactly one start event");

            if (start.Outgoing.Count() != 1)
                throw new ArgumentException("Process start needs to have exactly one outgoing connection");

            //Create my start element
            var myStart = new ContractStartEvent
            {
                Id = start.Id,
                Name = start.Name,
                Outgoing = ConstructNext(source, start.Outgoing.Single())
            };

            result.StartEvent = myStart;
            return result;
        }

        ContractProcessElement ConstructNext(Process source, string nextId)
        {
            throw new NotImplementedException();
        }

    }
}
