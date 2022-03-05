using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Abstraction.Data;
using DasContract.Abstraction.Processes;
using DasContract.Abstraction.Processes.Events;
using DasContract.Abstraction.Processes.Gateways;
using DasContract.Abstraction.Processes.Tasks;
using DasContract.Blockchain.Plutus.Data.Abstraction;
using DasContract.Blockchain.Plutus.Data.DasContractConversion.Processes.Activities.MultiInstance;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;
using DasContract.Blockchain.Plutus.Data.Processes.Process;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Activities;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Events;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Gateways;
using DasContract.Blockchain.Plutus.Data.Processes.Process.MultiInstances;
using DasContract.Blockchain.Plutus.Data.Users;

namespace DasContract.Blockchain.Plutus.Data.DasContractConversion.Processes
{
    public class ContractProcessConvertor : IConvertor<Process, ContractProcess>
    {
        private readonly IConvertor<ScriptTask, ContractScriptActivity> scriptConvertor;
        private readonly IConvertor<UserTask, ContractUserActivity> userConvertor;
        private readonly IConvertor<TimerBoundaryEvent, ContractTimerBoundaryEvent> timerBoundaryConvertor;
        private readonly IConvertor<ExclusiveGateway, ContractExclusiveGateway> exclusiveGatewayConvertor;
        private readonly IConvertor<ExclusiveGateway, ContractMergingExclusiveGateway> mergingExclusiveGatewayConvertor;

        public ContractProcessConvertor(
            IConvertor<ScriptTask, ContractScriptActivity> scriptConvertor,
            IConvertor<UserTask, ContractUserActivity> userConvertor,
            IConvertor<TimerBoundaryEvent, ContractTimerBoundaryEvent> timerBoundaryConvertor,
            IConvertor<ExclusiveGateway, ContractExclusiveGateway> exclusiveGatewayConvertor,
            IConvertor<ExclusiveGateway, ContractMergingExclusiveGateway> mergingExclusiveGatewayConvertor)
        {
            this.scriptConvertor = scriptConvertor;
            this.userConvertor = userConvertor;
            this.timerBoundaryConvertor = timerBoundaryConvertor;
            this.exclusiveGatewayConvertor = exclusiveGatewayConvertor;
            this.mergingExclusiveGatewayConvertor = mergingExclusiveGatewayConvertor;
        }

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

            //Script task
            if (currentElement is ScriptTask scriptTask)
            {
                var scriptResult = scriptConvertor.Convert(scriptTask);

                //Save
                knownElements.Add(scriptResult.Id, scriptResult);

                //Next
                var nextId = scriptTask.Outgoing.SingleOrDefault();
                if (nextId is null)
                    throw new Exception($"Script task {scriptTask.Id} should have exactly one output");
                scriptResult.Outgoing = ConstructNext(source, GetSequenceFlowIdTarget(source, nextId), knownElements);

                return scriptResult;
            }

            //User task
            else if (currentElement is UserTask userTask)
            {
                var userResult = userConvertor.Convert(userTask);

                //Save
                knownElements.Add(userResult.Id, userResult);

                //Timer boundary event
                var suitableTimerBoundaryEvents = source.Events
                    .OfType<TimerBoundaryEvent>()
                    .Where(e => e.AttachedTo == userTask.Id);
                if (suitableTimerBoundaryEvents.Count() > 1)
                    throw new Exception($"Only one timer boundary event at once is allowed ({userResult.Name})");
                else if (suitableTimerBoundaryEvents.Count() == 1)
                {
                    var suitableTimerBoundaryEvent = suitableTimerBoundaryEvents.Single();
                    var timerBoundaryResult = timerBoundaryConvertor.Convert(suitableTimerBoundaryEvent);
                    userResult.BoundaryEvents.Add(timerBoundaryResult);

                    //Next
                    var boundaryNextId = suitableTimerBoundaryEvent.Outgoing.SingleOrDefault();
                    if (boundaryNextId is null)
                        throw new Exception($"Timer boundary event {suitableTimerBoundaryEvent.Id} should have exactly one output");
                    timerBoundaryResult.TimeOutDirection = ConstructNext(source, GetSequenceFlowIdTarget(source, boundaryNextId), knownElements);
                }


                //Next
                var nextId = userTask.Outgoing.SingleOrDefault();
                if (nextId is null)
                    throw new Exception($"User task {userTask.Id} should have exactly one output");
                userResult.Outgoing = ConstructNext(source, GetSequenceFlowIdTarget(source, nextId), knownElements);

                return userResult;
            }

            //Exclusive gateway
            else if (currentElement is ExclusiveGateway exclusiveGateway)
            {
                //Merging exclusive gateway
                if (exclusiveGateway.Outgoing.Count() == 1)
                {
                    var gatewayResult = mergingExclusiveGatewayConvertor.Convert(exclusiveGateway);

                    //Save
                    knownElements.Add(gatewayResult.Id, gatewayResult);

                    //Next
                    var nextId = exclusiveGateway.Outgoing.SingleOrDefault();
                    gatewayResult.Outgoing = ConstructNext(source, GetSequenceFlowIdTarget(source, nextId), knownElements);

                    return gatewayResult;
                }
                //Branching exclusive gateway
                else if (exclusiveGateway.Outgoing.Count() > 1)
                {
                    var gatewayResult = exclusiveGatewayConvertor.Convert(exclusiveGateway);

                    //Save
                    knownElements.Add(gatewayResult.Id, gatewayResult);

                    //Next
                    var nextIds = exclusiveGateway.Outgoing;
                    gatewayResult.Outgoing = nextIds.Select(e =>
                    {
                        if (!source.SequenceFlows.ContainsKey(e))
                            throw new Exception($"Invalid sequence flow id {e}");
                        var sequenceFlow = source.SequenceFlows[e];
                        var targetId = sequenceFlow.TargetId;
                        if(!source.ProcessElements.ContainsKey(targetId))
                            throw new Exception($"Invalid sequence flow target id {targetId}");

                        var targetElement = source.ProcessElements[targetId];
                        var targetCondition = sequenceFlow.Condition;

                        return new ContractConditionedConnection()
                        {
                            Condition = targetCondition,
                            Target = ConstructNext(source, targetElement, knownElements),
                        };
                    }).ToList();

                    return gatewayResult;
                }
                else
                    throw new Exception($"Exclusive gateway {exclusiveGateway.Id} has zero outputs");

            }

            //End task
            else if (currentElement is EndEvent endEvent)
            {
                var endResult = new ContractEndEvent()
                {
                    Id = endEvent.Id,
                };

                //Save
                knownElements.Add(endResult.Id, endResult);

                return endResult;
            }

            throw new Exception($"Unhandled type of process element: {currentElement.GetType().Name}");
        }

        public static ContractProcess Bind(ContractProcess process, 
            IEnumerable<ContractUser> users, 
            IEnumerable<ContractRole> roles)
        {
            var userActivities = process.ProcessElements.OfType<ContractUserActivity>();
            foreach(var userActivity in userActivities) 
                ContractUserActivityConvertor.Bind(userActivity, users, roles);

            return process;
        }
    }
}
