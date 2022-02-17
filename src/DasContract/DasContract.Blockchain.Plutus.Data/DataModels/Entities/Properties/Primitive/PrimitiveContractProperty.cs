using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive
{
    public class PrimitiveContractProperty : ContractProperty
    {
        /// <summary>
        /// Data type of this property
        /// </summary>
        public PrimitiveContractPropertyType Type { get; set; }

        /// <inheritdoc/>
        public override T Accept<T>(IContractPropertyVisitor<T> visitor)
            => visitor.Visit(this);

    }
}
