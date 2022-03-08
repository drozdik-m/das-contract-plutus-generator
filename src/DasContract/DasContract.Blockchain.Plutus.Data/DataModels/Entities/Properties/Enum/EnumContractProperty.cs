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

        /// <summary>
        /// Id of the referenced enum
        /// </summary>
        public string EnumEntityId { get; set; } = string.Empty;

        /// <inheritdoc/>
        public override T Accept<T>(IContractPropertyVisitor<T> visitor)
            => visitor.Visit(this);

    }
}
