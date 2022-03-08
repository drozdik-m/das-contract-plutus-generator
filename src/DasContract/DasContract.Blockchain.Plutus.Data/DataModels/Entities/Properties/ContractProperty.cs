using System;
using System.Collections.Generic;
using DasContract.Blockchain.Plutus.Data.Interfaces;


namespace DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties
{
    public abstract class ContractProperty : IIdentifiable, INamable
    {
        /// <inheritdoc/>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <inheritdoc/>
        public string Name => Id;

        /// <summary>
        /// Cardinality of this property - collection or single value?
        /// </summary>
        public virtual ContractPropertyCardinality Cardinality { get; set; } = ContractPropertyCardinality.Single;

        /// <summary>
        /// True if this property is mandatory, else false
        /// </summary>
        public bool IsMandatory { get; set; } = false;

        /// <summary>
        /// Collects entity dependencies of this property
        /// </summary>
        /// <param name="dependencies">The list of dependencies</param>
        public virtual void CollectDependencies(ref Dictionary<string, ContractEntity> dependencies)
        {
            
        }

        /// <summary>
        /// Accepts a property visitor
        /// </summary>
        /// <param name="visitor"></param>
        public abstract T Accept<T>(IContractPropertyVisitor<T> visitor); 
    }
}
