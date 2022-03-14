using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process
{
    /// <summary>
    /// Element that is part of the contract process (can be event, activity, ...)
    /// </summary>
    public abstract class ContractProcessElement : IIdentifiable, INamable
    {
        /// <inheritdoc/>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <inheritdoc/>
        public string Name => Id;

        /// <summary>
        /// Collects all successor process elements if this element
        /// </summary>
        /// <param name="collector"></param>
        public virtual void CollectSuccessors(ref Dictionary<string, ContractProcessElement> collector)
        {
            if (collector.ContainsKey(Id))
                return;

            collector.TryAdd(Id, this);
        }

        /// <summary>
        /// Accepts a process element visitor
        /// </summary>
        /// <param name="visitor"></param>
        public abstract T Accept<T>(IContractProcessElementVisitor<T> visitor);
    }
}
