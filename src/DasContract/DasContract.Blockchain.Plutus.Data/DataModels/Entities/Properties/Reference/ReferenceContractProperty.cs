﻿using DasContract.Blockchain.Plutus.Data.DataModels.Entities;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Reference
{
    public class ReferenceContractProperty : ContractProperty
    {
        /// <summary>
        /// Reference to the entity of which this property is data type of 
        /// </summary>
        public ContractEntity Entity { get; set; }

        /// <summary>
        /// Id if the linked entity 
        /// </summary>
        public string EntityId { get; set; } = string.Empty;

        /// <inheritdoc/>
        public override void CollectDependencies(ref Dictionary<string, ContractEntity> dependencies)
        {
            base.CollectDependencies(ref dependencies);
            dependencies.TryAdd(Entity.Id, Entity);
        }

        /// <inheritdoc/>
        public override T Accept<T>(IContractPropertyVisitor<T> visitor)
            => visitor.Visit(this);

    }
}
