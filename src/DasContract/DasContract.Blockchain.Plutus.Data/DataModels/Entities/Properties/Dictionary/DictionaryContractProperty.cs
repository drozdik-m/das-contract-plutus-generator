using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Dictionary
{
    public class DictionaryContractProperty : ContractProperty
    {
        /// <summary>
        /// Key data type of this property
        /// </summary>
        public PrimitiveContractPropertyType KeyType { get; set; }

        /// <summary>
        /// Value data type of this property
        /// </summary>
        public ContractProperty ValueType { get; set; }

        /// <inheritdoc/>
        public override T Accept<T>(IContractPropertyVisitor<T> visitor)
            => visitor.Visit(this);

        /// <inheritdoc/>
        public override void CollectDependencies(ref Dictionary<string, ContractEntity> dependencies)
        {
            base.CollectDependencies(ref dependencies);
            ValueType.CollectDependencies(ref dependencies);
        }
    }
}
