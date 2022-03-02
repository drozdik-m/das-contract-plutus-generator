using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Abstraction.Data;
using DasContract.Abstraction.Processes;
using DasContract.Abstraction.Processes.Events;
using DasContract.Abstraction.Processes.Tasks;
using DasContract.Blockchain.Plutus.Data.Abstraction;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;
using DasContract.Blockchain.Plutus.Data.Processes.Process;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Activities;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Events;
using DasContract.Blockchain.Plutus.Data.Processes.Process.MultiInstances;

namespace DasContract.Blockchain.Plutus.Data.DasContractConversion.Processes
{
    public class ProcessConvertor : IConvertor<Process, ContractProcess>
    {
        /// <inheritdoc/>
        public ContractProcess Convert(Process source)
        {
            var result = new ContractProcess
            {
                IsMain = source.IsExecutable,
            };

            //Start element
            var start = source.ProcessElements.Values.OfType<StartEvent>().SingleOrDefault();
            if (start is null)
                throw new ArgumentException("Process needs to have exactly one start event");

            if (start.Outgoing.Count() != 1)
                throw new ArgumentException("Process start needs to have exactly one outgoing connection");

            //Create my start element
            var myStart = new ContractStartEvent
            {
                Id = start.Id,
            };
            var knownElements = new Dictionary<string, ContractProcessElement>
            {
                { myStart.Id, myStart }
            };
            myStart.Outgoing = ConstructNext(source, start, knownElements);

            result.StartEvent = myStart;
            return result;
        }

        /// <summary>
        /// Recursively constructs an element and its outputs
        /// </summary>
        /// <param name="source"></param>
        /// <param name="currentElement"></param>
        /// <param name="knownElements"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        ContractProcessElement ConstructNext(Process source, 
            ProcessElement currentElement, 
            Dictionary<string, ContractProcessElement> knownElements)
        {
            //Check if the element is already known
            if (knownElements.ContainsKey(currentElement.Id))
                return knownElements[currentElement.Id];

            ContractProcessElement result = null;

            if (currentElement is ScriptTask scriptTask)
            {
                var scriptResult = new ContractScriptActivity()
                {
                    Id = scriptTask.Id,
                    Code = scriptTask.Script
                };

                if (scriptTask.InstanceType == InstanceType.Sequential)
                {
                    scriptResult.MultiInstance = new ContractSequentialMultiInstance()
                    {
                        LoopCardinality = scriptTask.LoopCardinality.ToString(),
                    };
                }
                else if (scriptTask.InstanceType == InstanceType.Parallel)
                    throw new Exception($"Parallel multiinstances are not supporter ({scriptTask.Id})");

                result = scriptResult;
            }
            else if (currentElement is EndEvent endEvent)
            {
                var endResult = new ContractEndEvent()
                {
                    Id = endEvent.Id,
                };

                result = endResult;
            }


            //Add to known elements
            if (result is null)
                throw new Exception($"Unhandled type of process element: {currentElement.GetType().Name}");
            else
                knownElements.Add(result.Id, result);

            return result;
        }

    }
}
