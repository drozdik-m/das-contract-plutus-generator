using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using DasContract.Editor.Entities.Processes.Process.Activities;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process
{
    public abstract class ContractProcessElement : IIdentifiable, INamable
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; } = string.Empty;
    }
}
