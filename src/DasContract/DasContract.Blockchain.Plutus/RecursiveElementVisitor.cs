using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Blockchain.Plutus.Code;
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

        /// <summary>
        /// Checks if the element has already been visited.
        /// Automatically adds the element to visited elements if required.
        /// </summary>
        /// <param name="element"></param>
        /// <returns>Returns true if the element has not been visited. Else false.</returns>
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

        /// <inheritdoc/>
        public abstract IPlutusCode Visit(ContractExclusiveGateway element);

        /// <inheritdoc/>
        public abstract IPlutusCode Visit(ContractMergingExclusiveGateway element);

        /// <inheritdoc/>
        public abstract IPlutusCode Visit(ContractStartEvent element);

        /// <inheritdoc/>
        public abstract IPlutusCode Visit(ContractEndEvent element);

        /// <inheritdoc/>
        public abstract IPlutusCode Visit(ContractCallActivity element);

        /// <inheritdoc/>
        public abstract IPlutusCode Visit(ContractUserActivity element);

        /// <inheritdoc/>
        public abstract IPlutusCode Visit(ContractScriptActivity element);

        /// <inheritdoc/>
        public abstract IPlutusCode Visit(ContractTimerBoundaryEvent contractTimerBoundaryEvent);
    }
}

