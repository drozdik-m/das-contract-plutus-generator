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
    public class TxTypeVisitor : IContractProcessElementVisitor<TxType>
    {

        public bool IsSubprocess { get; }
        public TxTypeVisitor(bool isSubprocess)
        {
            IsSubprocess = isSubprocess;
        }

        public TxType Visit(ContractExclusiveGateway element) => TxType.NonTx;

        public TxType Visit(ContractMergingExclusiveGateway element) => TxType.NonTx;

        public TxType Visit(ContractStartEvent element) => TxType.Implicit;

        public TxType Visit(ContractEndEvent element) => IsSubprocess ? TxType.NonTx : TxType.Tx;

        public TxType Visit(ContractCallActivity element) => TxType.NonTx;

        public TxType Visit(ContractUserActivity element) => IsSequential(element) ? TxType.NonTx : TxType.Tx;

        public TxType Visit(ContractScriptActivity element) => TxType.NonTx;

        public TxType Visit(ContractTimerBoundaryEvent contractTimerBoundaryEvent) => TxType.Tx;


        bool IsSequential(ContractActivity activity) => activity.MultiInstance is ContractSequentialMultiInstance;
    }
}
