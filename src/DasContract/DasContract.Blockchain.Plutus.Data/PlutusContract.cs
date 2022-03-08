using DasContract.Blockchain.Plutus.Data.DataModels;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using DasContract.Blockchain.Plutus.Data.Processes;
using DasContract.Blockchain.Plutus.Data.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace DasContract.Blockchain.Plutus.Data
{
    /// <summary>
    /// Plutus contract 
    /// </summary>
    public class PlutusContract : IIdentifiable, INamable
    {
        /// <inheritdoc/>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <inheritdoc/>
        public string Name => Id;

        /// <summary>
        /// Code for validation of the whole contract
        /// </summary>
        public string GlobalValidationCode { get; set; } = string.Empty;

        /// <summary>
        /// Individual lines for validation of the whole contract
        /// </summary>
        public IEnumerable<string> GlobalValidationCodeLines
        {
            get
            {
                using var reader = new StringReader(GlobalValidationCode);
                List<string> result = new List<string>();
                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                    result.Add(line);
                return result;
            }
        }

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
