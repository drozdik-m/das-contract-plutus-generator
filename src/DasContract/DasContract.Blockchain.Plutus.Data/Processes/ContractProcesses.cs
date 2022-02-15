using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using DasContract.Blockchain.Plutus.Data.Processes.Process;
using DasContract.Editor.Entities.Processes.Diagrams;

namespace DasContract.Blockchain.Plutus.Data.Processes
{
    public class ContractProcesses 
    {

        /// <summary>
        /// BPMN 2.0 XML with process description and a visual process information
        /// </summary>
        public BPMNProcessDiagram Diagram { get; set; } = BPMNProcessDiagram.Default();

        /// <summary>
        /// List of processes
        /// </summary>
        public ICollection<ContractProcess> Processes { get; set; } = new List<ContractProcess>();

        /// <summary>
        /// The main process where the contract starts and ends
        /// </summary>
        public ContractProcess Main => Processes.Where(e => e.IsMain).Single();

        /// <summary>
        /// Processes without the main process
        /// </summary>
        public IEnumerable<ContractProcess> Subprocesses => Processes.Where(e => !e.IsMain);
    }
}
