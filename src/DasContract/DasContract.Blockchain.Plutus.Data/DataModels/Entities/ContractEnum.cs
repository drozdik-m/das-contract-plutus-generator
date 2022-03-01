using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Data.DataModels.Entities
{
    public class ContractEnum: INamable, IIdentifiable
    {
        /// <inheritdoc/>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <inheritdoc/>
        public string Name => Id;

        /// <summary>
        /// Values of this enum
        /// </summary>
        public ICollection<string> Values { get; set; } = new List<string>();

        
    }
}
