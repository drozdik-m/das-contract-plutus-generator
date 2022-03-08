using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;

namespace DasContract.Blockchain.Plutus.Data.Forms
{
    /// <summary>
    /// Form for users to fill out
    /// </summary>
    public class ContractForm
    {
        /// <summary>
        /// Fields of this form
        /// </summary>
        public ICollection<PrimitiveContractProperty> Fields { get; set; } = new List<PrimitiveContractProperty>();
    }
}
