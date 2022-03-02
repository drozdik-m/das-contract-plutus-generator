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
    public class ContractProcessConvertor : IConvertor<Process, ContractProcess>
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
            var nextId = start.Outgoing.Single();
            var next = GetSequenceFlowIdTarget(source, nextId);
            myStart.Outgoing = ConstructNext(source, next, knownElements);

            result.StartEvent = myStart;
            return result;
        }

        /// <summary>
        /// Takes sequence flow id and finds out the target element
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sequenceFlowId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        ProcessElement GetSequenceFlowIdTarget(Process source, string sequenceFlowId)
        {
            if (!source.SequenceFlows.ContainsKey(sequenceFlowId))
                throw new Exception($"Invalid sequence flow id {sequenceFlowId}");

            var sequenceFlow = source.SequenceFlows[sequenceFlowId];
            var targetId = sequenceFlow.TargetId;

            if (!source.ProcessElements.ContainsKey(targetId))
                throw new Exception($"Invalid sequence flow target id {targetId}");

            var targetElement = source.ProcessElements[targetId];
            return targetElement;
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

            ContractProcessElement? result = default;

            //Script task
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
                    throw new Exception($"Parallel multiinstances are not supported ({scriptTask.Id})");

                var nextId = scriptTask.Outgoing.SingleOrDefault();
                if (nextId is null)
                    throw new Exception($"Script task {scriptTask.Id} should have exactly one output");
                scriptResult.Outgoing = ConstructNext(source, GetSequenceFlowIdTarget(source, nextId), knownElements);

                result = scriptResult;
            }

            //End task
            else if (currentElement is EndEvent endEvent)
            {
                var endResult = new ContractEndEvent()
                {
                    Id = endEvent.Id,
                };

                result = endResult;
            }

            else
                throw new Exception($"Unhandled type of process element: {currentElement.GetType().Name}");

            return result;
        }

    }
}
