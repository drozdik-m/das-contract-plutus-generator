using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DasContract.Blockchain.Plutus.Data;
using DasContract.Blockchain.Plutus.Data.DataModels;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities;
using DasContract.Blockchain.Plutus.Data.Processes;
using DasContract.Blockchain.Plutus.Data.Processes.Process;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Activities;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Events;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Gateways;
using DasContract.Blockchain.Plutus.Data.Processes.Process.MultiInstances;

namespace DasContract.Blockchain.Plutus.Console.Tests.DemoContracts
{
    public static class TransitionDemoContract
    {
        public static PlutusContract Get()
        {
            //--- SUBPROCESS 2 ---
            var sub2Start = new ContractStartEvent() { Id = "StartEventSub2" };
            var sub2End = new ContractEndEvent() { Id = "EndEventSub2" };

            sub2Start.Outgoing = sub2End;

            var subprocess2 = new ContractProcess()
            {
                Id = "Subprocess2",
                StartEvent = sub2Start,
            };

            //--- SUBPROCESS 1 ---
            var sub1Start = new ContractStartEvent() { Id = "StartEventSub1" };
            var sub1End = new ContractEndEvent() { Id = "EndEventSub1" };

            var scriptSub1 = new ContractScriptActivity() { Id = "Script1Sub" };

            var callSub2 = new ContractCallActivity()
            {
                Id = "Script1Sub",
                CalledProcess = subprocess2,
            };

            sub1Start.Outgoing = callSub2;
            callSub2.Outgoing = scriptSub1;
            scriptSub1.Outgoing = sub1End;

            var subprocess1 = new ContractProcess()
            {
                Id = "Subprocess1",
                StartEvent = sub1Start,
            };

            //--- MAIN PROCESS ---
            var mainStart = new ContractStartEvent() { Id = "StartEvent" };
            var mainEnd = new ContractEndEvent() { Id = "EndEvent" };

            var script1 = new ContractScriptActivity()
            {
                Id = "Script1",
            };

            var script1Loop = new ContractScriptActivity()
            {
                Id = "Script1Loop",
                Code = ContractScriptActivity.TransitionPragma + Environment.NewLine +
                       "ahoj to je kod" + Environment.NewLine +
                       "\t ano vskutku" + Environment.NewLine +
                       "yep" + Environment.NewLine,
                MultiInstance = new ContractSequentialMultiInstance() { LoopCardinality = "3" },
            };

            var user1 = new ContractUserActivity()
            {
                Id = "User1",
                Code = ContractUserActivity.ConstrainsPragma + Environment.NewLine +
                       "Toto je constraint" + Environment.NewLine +
                       ContractUserActivity.ExpectedValuePragma + Environment.NewLine +
                       "Toto je expected value" + Environment.NewLine +
                       ContractUserActivity.TransitionPragma + Environment.NewLine +
                       "Toto je transition" + Environment.NewLine +
                       ContractUserActivity.FormValidationPragma + Environment.NewLine +
                       "Toto je form validation" + Environment.NewLine +
                       ContractUserActivity.NewValuePragma + Environment.NewLine +
                       "Toto je new value" + Environment.NewLine,
            };

            var user1Loop = new ContractUserActivity()
            {
                Id = "User1Loop",
                Code = user1.Code,
                MultiInstance = new ContractSequentialMultiInstance() { LoopCardinality = "3" },
            };

            var user1LoopTimer = new ContractUserActivity()
            {
                Id = "User1Loop",
                Code = user1.Code,
                MultiInstance = new ContractSequentialMultiInstance() { LoopCardinality = "4" },
                BoundaryEvents = new ContractBoundaryEvent[]
                {
                    new ContractTimerBoundaryEvent()
                    {
                        Id = "User1LoopBoundaryTimer",
                        TimerDefinition = "zejtra",
                        TimeOutDirection = mainEnd
                    }
                }
            };

            var user1Timer = new ContractUserActivity()
            {
                Id = "User1Loop",
                Code = user1.Code,
                BoundaryEvents = new ContractBoundaryEvent[]
                {
                    new ContractTimerBoundaryEvent()
                    {
                        Id = "User1BoundaryTimer",
                        TimerDefinition = "včera",
                        TimeOutDirection = mainEnd,
                    }
                }
            };

            var exclusiveGateway1 = new ContractExclusiveGateway() { Id = "ExclusiveGateway1" };
            var mergingGateway1 = new ContractMergingExclusiveGateway() { Id = "MergingGateway1" };

            var callActivitySub1 = new ContractCallActivity()
            {
                Id = "CallSubprocess1",
                CalledProcess = subprocess1,
            };

            var callActivitySub1Loop = new ContractCallActivity()
            {
                Id = "CallSubprocess1Loop",
                CalledProcess = subprocess1,
                MultiInstance = new ContractSequentialMultiInstance() { LoopCardinality = "9" }
            };

            mainStart.Outgoing = callActivitySub1;
            callActivitySub1.Outgoing = script1;
            script1.Outgoing = mainEnd;

            /*mainStart.Outgoing = exclusiveGateway1;
            exclusiveGateway1.Outgoing = new List<ContractConditionedConnection>()
            {
                new ContractConditionedConnection() { Condition = "condition1", Target = script1 },
                new ContractConditionedConnection() { Condition = "condition2", Target = script1Loop },
                new ContractConditionedConnection() { Condition = "condition3", Target = user1 },
                new ContractConditionedConnection() { Condition = "condition4", Target = user1Loop },
                new ContractConditionedConnection() { Condition = "condition5", Target = user1LoopTimer },
                new ContractConditionedConnection() { Condition = "condition6", Target = user1Timer },
            };
            script1.Outgoing = mergingGateway1;
            script1Loop.Outgoing = mergingGateway1;
            user1.Outgoing = mergingGateway1;
            user1Loop.Outgoing = mergingGateway1;
            user1LoopTimer.Outgoing = mergingGateway1;
            user1Timer.Outgoing = mergingGateway1;
            mergingGateway1.Outgoing = mainEnd;*/

            var mainProcess = new ContractProcess()
            {
                IsMain = true,
                StartEvent = mainStart,
            };

            //--- CONTRACT ---
            var contract = new PlutusContract()
            {
                 Processes = new ContractProcesses()
                 {
                       AllProcesses = new ContractProcess[]
                       {
                           mainProcess,
                       }
                 },
                 DataModel = new ContractDataModel()
                 {
                     Entities = new ContractEntity[]
                     {
                         new ContractEntity()
                         {
                             Id = "RootEntity",
                             IsRoot = true,
                         }
                     }
                 }
            };
            
            return contract;    
        }
    }
}
