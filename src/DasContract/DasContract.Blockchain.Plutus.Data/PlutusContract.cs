using DasContract.Blockchain.Plutus.Data.DataModels;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using DasContract.Blockchain.Plutus.Data.Processes;
using DasContract.Blockchain.Plutus.Data.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DasContract.Blockchain.Plutus.Data
{
    public class PlutusContract : IIdentifiable, INamable
    {
        /// <inheritdoc/>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <inheritdoc/>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Data model of this contract
        /// </summary>
        public ContractDataModel DataModel { get; set; } = new ContractDataModel();

        /// <summary>
        /// Processes of this contract
        /// </summary>
        public ContractProcesses Processes { get; set; } = new ContractProcesses();

        /// <summary>
        /// Users of this contract
        /// </summary>
        public ContractUsers Identities { get; set; } = new ContractUsers();
    }

}
