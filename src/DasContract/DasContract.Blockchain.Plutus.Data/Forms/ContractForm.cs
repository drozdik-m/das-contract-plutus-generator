using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;

namespace DasContract.Blockchain.Plutus.Data.Forms
{
    public class ContractForm
    {
        public ICollection<PrimitiveContractProperty> Fields { get; set; } = new List<PrimitiveContractProperty>();
    }
}
