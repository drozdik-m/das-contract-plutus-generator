using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Enum
{
    public class EnumContractProperty : ContractProperty
    {
        /// <summary>
        /// Reference to the enum
        /// </summary>
        public ContractEnum EnumEntity { get; set; }

        /// <inheritdoc/>
        public override T Accept<T>(IContractPropertyVisitor<T> visitor)
            => visitor.Visit(this);

    }
}
