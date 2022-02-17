using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process
{
    public abstract class ContractProcessElement : IIdentifiable, INamable
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name => Id;

        public virtual void CollectSuccessors(ref Dictionary<string, ContractProcessElement> collector)
        {
            collector.TryAdd(Id, this);
        }
    }
}
