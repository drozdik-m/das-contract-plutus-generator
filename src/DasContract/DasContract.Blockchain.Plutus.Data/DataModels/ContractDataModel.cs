﻿using DasContract.Blockchain.Plutus.Data.DataModels.Entities;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DasContract.Blockchain.Plutus.Data.DataModels
{
    public class ContractDataModel //: IIdentifiable
    {
        /// <inheritdoc/>
        //public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Collection of entities contained in this model
        /// </summary>
        public ICollection<ContractEntity> Entities { get; set; } = new List<ContractEntity>();

        /// <summary>
        /// Collection of enums contained in this model
        /// </summary>
        public ICollection<ContractEnum> Enums { get; set;} = new List<ContractEnum>();

        /// <summary>
        /// Returns the root entity of this data model
        /// </summary>
        public ContractEntity RootEntity => Entities
            .Where(e => e.IsRoot)
            .Single();

        /// <summary>
        /// Returns all entities except the root one
        /// </summary>
        public IEnumerable<ContractEntity> NonRootEntities => Entities
            .Where(e => !e.IsRoot);

        /// <summary>
        /// Adds a new entity to this data model
        /// </summary>
        /// <param name="newEntity"></param>
        public void AddEntity(ContractEntity newEntity)
        {
            Entities.Add(newEntity);
        }

        /// <summary>
        /// Removes an entity from this data model
        /// </summary>
        /// <param name="removeEntity"></param>
        public void RemoveEntity(ContractEntity removeEntity)
        {
            Entities.Remove(removeEntity);
        }

        /// <summary>
        /// Adds a new enum to this data model
        /// </summary>
        /// <param name="newEum"></param>
        public void AddEnum(ContractEnum newEum)
        {
            Enums.Add(newEum);
        }

        /// <summary>
        /// Removes an enum from this data model
        /// </summary>
        /// <param name="removedEnum"></param>
        public void RemoveEnum(ContractEnum removedEnum)
        {
            Enums.Remove(removedEnum);
        }
    }
}
