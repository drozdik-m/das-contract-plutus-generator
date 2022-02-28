using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using DasContract.Blockchain.Plutus.Data.Processes.Process;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Activities;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Events;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Gateways;

namespace DasContract.Blockchain.Plutus
{
    public abstract class RecursiveElementVisitor : IContractProcessElementVisitor<IPlutusCode>
    {
        protected HashSet<string> VisitedElements { get; } = new HashSet<string>();

        protected bool TryVisit(INamable element)
        {
            var result = false;
            if (!VisitedElements.Contains(element.Name))
            {
                result = true;
                VisitedElements.Add(element.Name);
            }

            return result;
        }

        public abstract IPlutusCode Visit(ContractExclusiveGateway element);
        public abstract IPlutusCode Visit(ContractMergingExclusiveGateway element);
        public abstract IPlutusCode Visit(ContractStartEvent element);
        public abstract IPlutusCode Visit(ContractEndEvent element);
        public abstract IPlutusCode Visit(ContractCallActivity element);
        public abstract IPlutusCode Visit(ContractUserActivity element);
        public abstract IPlutusCode Visit(ContractScriptActivity element);
        public abstract IPlutusCode Visit(ContractTimerBoundaryEvent contractTimerBoundaryEvent);
    }
}
