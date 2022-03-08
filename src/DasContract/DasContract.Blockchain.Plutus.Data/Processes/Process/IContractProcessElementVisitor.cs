using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Dictionary;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Activities;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Events;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Gateways;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process
{
    public interface IContractProcessElementVisitor<T>
    {
        /// <inheritdoc/>
        T Visit(ContractExclusiveGateway element);

        /// <inheritdoc/>
        T Visit(ContractMergingExclusiveGateway element);

        /// <inheritdoc/>
        T Visit(ContractStartEvent element);

        /// <inheritdoc/>
        T Visit(ContractEndEvent element);

        /// <inheritdoc/>
        T Visit(ContractCallActivity element);

        /// <inheritdoc/>
        T Visit(ContractUserActivity element);

        /// <inheritdoc/>
        T Visit(ContractScriptActivity element);

        /// <inheritdoc/>
        T Visit(ContractTimerBoundaryEvent contractTimerBoundaryEvent);
    }
}
