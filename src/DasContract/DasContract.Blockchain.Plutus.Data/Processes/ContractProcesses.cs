using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using DasContract.Blockchain.Plutus.Data.Processes.Process;
using DasContract.Editor.Entities.Processes.Diagrams;
using DasContract.Editor.Entities.Processes.Factories;
using DasContract.Editor.Entities.Processes.Process.Activities;

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
    }
}
