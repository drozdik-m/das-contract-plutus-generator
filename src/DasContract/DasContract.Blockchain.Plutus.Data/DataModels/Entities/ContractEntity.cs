using System.Collections.Generic;
using System.Xml.Serialization;
using System;
using DasContract.Editor.Entities.DataModels.Entities.Properties.Reference;
using System.Linq;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;

namespace DasContract.Blockchain.Plutus.Data.DataModels.Entities
{
    public class ContractEntity : IIdentifiable, INamable
    {
        /// <inheritdoc/>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <inheritdoc/>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// True if this entity is the root entity, else false
        /// </summary>
        public bool IsRoot { get; set; } = false;

        /// <summary>
        /// Collection of primitive properties of this entity
        /// </summary>
        public ICollection<PrimitiveContractProperty> PrimitiveProperties { get; set; } = new List<PrimitiveContractProperty>();

        /// <summary>
        /// Collection of reference properties of this entity
        /// </summary>
        public ICollection<ReferenceContractProperty> ReferenceProperties { get; set; } = new List<ReferenceContractProperty>();

        /// <summary>
        /// Collection of all properties of this entity
        /// </summary>
        public IEnumerable<ContractProperty> Properties => new List<ContractProperty>()
            .Concat(PrimitiveProperties)
            .Concat(ReferenceProperties)
            .ToList();

        /// <summary>
        /// Adds a property from this entity
        /// </summary>
        /// <param name="newProperty"></param>
        public void AddProperty(PrimitiveContractProperty newProperty)
        {
            PrimitiveProperties.Add(newProperty);
        }

        /// <summary>
        /// Removes a property from this entity
        /// </summary>
        /// <param name="removeProperty"></param>
        public void RemoveProperty(PrimitiveContractProperty removeProperty)
        {
            PrimitiveProperties.Remove(removeProperty);
        }

        /// <summary>
        /// Adds a property from this entity
        /// </summary>
        /// <param name="newProperty"></param>
        public void AddProperty(ReferenceContractProperty newProperty)
        {
            ReferenceProperties.Add(newProperty);
        }

        /// <summary>
        /// Removes a property from this entity
        /// </summary>
        /// <param name="removeProperty"></param>
        public void RemoveProperty(ReferenceContractProperty removeProperty)
        {
            ReferenceProperties.Remove(removeProperty);
        }
    }
}
