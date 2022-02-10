using System;
using DasContract.Blockchain.Plutus.Data.Interfaces;


namespace DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties
{
    public abstract class ContractProperty : IIdentifiable, INamable
    {
        /// <inheritdoc/>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <inheritdoc/>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Cardinality of this property - collection of single value?
        /// </summary>
        public ContractPropertyCardinality Cardinality { get; set; } = ContractPropertyCardinality.Single;

        /// <summary>
        /// True if this property is mandatory, else false
        /// </summary>
        public bool IsMandatory { get; set; } = false;
    }
}
