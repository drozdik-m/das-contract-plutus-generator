using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using DasContract.Blockchain.Plutus.Data.Processes.Process;

namespace DasContract.Blockchain.Plutus.Data.Processes
{
    public class ContractProcesses 
    {
        /// <summary>
        /// List of processes
        /// </summary>
        public ICollection<ContractProcess> AllProcesses { get; set; } = new List<ContractProcess>();

        /// <summary>
        /// The main process where the contract starts and ends
        /// </summary>
        public ContractProcess Main => AllProcesses.Where(e => e.IsMain).Single();

        /// <summary>
        /// Processes without the main process
        /// </summary>
        public IEnumerable<ContractProcess> Subprocesses => AllProcesses.Where(e => !e.IsMain);
    }
}
