using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using DasContract.Blockchain.Plutus.Data.Processes.Process;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Activities;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Events;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Gateways;
using System.Linq;
using DasContract.Blockchain.Plutus.Code.Comments;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Transitions.NonTx;
using DasContract.Blockchain.Plutus.Data.Processes.Process.MultiInstances;

namespace DasContract.Blockchain.Plutus.Transitions
{
    /// <summary>
    /// Visitor that figues out the TxType if a process element 
    /// </summary>
    public class TxTypeVisitor : IContractProcessElementVisitor<TxType>
    {
        public bool IsSubprocess { get; }

        public TxTypeVisitor(bool isSubprocess)
        {
            IsSubprocess = isSubprocess;
        }

        /// <inheritdoc/>
        public TxType Visit(ContractExclusiveGateway element) => TxType.NonTx;

        /// <inheritdoc/>
        public TxType Visit(ContractMergingExclusiveGateway element) => TxType.NonTx;

        /// <inheritdoc/>
        public TxType Visit(ContractStartEvent element) => TxType.Implicit;

        /// <inheritdoc/>
        public TxType Visit(ContractEndEvent element) => IsSubprocess ? TxType.NonTx : TxType.Tx;

        /// <inheritdoc/>
        public TxType Visit(ContractCallActivity element) => TxType.NonTx;

        /// <inheritdoc/>
        public TxType Visit(ContractUserActivity element) => IsSequential(element) ? TxType.NonTx : TxType.Tx;

        /// <inheritdoc/>
        public TxType Visit(ContractScriptActivity element) => TxType.NonTx;

        /// <inheritdoc/>
        public TxType Visit(ContractTimerBoundaryEvent contractTimerBoundaryEvent) => TxType.Tx;

        /// <summary>
        /// Checks is the contract activity is a sequential multi instance activity
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        bool IsSequential(ContractActivity activity) => activity.MultiInstance is ContractSequentialMultiInstance;
    }
}
