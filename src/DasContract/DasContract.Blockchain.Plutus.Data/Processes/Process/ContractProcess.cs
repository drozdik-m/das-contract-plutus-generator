using DasContract.Blockchain.Plutus.Data.Interfaces;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process
{
    public class ContractProcess : IIdentifiable, INamable
    {
        /// <inheritdoc/>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name => Id;

        public ContractStartEvent StartEvent { get; set; }

        public bool IsMain { get; set; } = false;

        public IEnumerable<ContractProcessElement> ProcessElements
        {
            get
            {
                var dictionary = new Dictionary<string, ContractProcessElement>();
                StartEvent.CollectSuccessors(ref dictionary);
                return dictionary.Values;
            }
        }

        

        public static ContractProcess Empty()
        {
            return new ContractProcess();
        }
    }
}
