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

        /// <inheritdoc/>
        public string Name => Id;

        /// <summary>
        /// That start event of this contract process
        /// </summary>
        public ContractStartEvent? StartEvent { get; set; }

        /// <summary>
        /// If this process is the first and main process of the contract, then true.
        /// Else false.
        /// Only one main contract is possible.
        /// </summary>
        public bool IsMain { get; set; } = false;

        /// <summary>
        /// Returns all process elements of this process
        /// </summary>
        public IEnumerable<ContractProcessElement> ProcessElements
        {
            get
            {
                var dictionary = new Dictionary<string, ContractProcessElement>();
                StartEvent.CollectSuccessors(ref dictionary);
                return dictionary.Values;
            }
        }
    }
}
