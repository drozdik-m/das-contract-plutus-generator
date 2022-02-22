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

namespace DasContract.Blockchain.Plutus.Transitions
{
    public class TxTypeVisitor : IContractProcessElementVisitor<TxType>
    {
        public TxType Visit(ContractExclusiveGateway element) => TxType.NonTx;

        public TxType Visit(ContractMergingExclusiveGateway element) => TxType.NonTx;

        public TxType Visit(ContractStartEvent element) => TxType.Implicit;

        public TxType Visit(ContractEndEvent element) => TxType.Tx;

        public TxType Visit(ContractCallActivity element) => TxType.Call;

        public TxType Visit(ContractUserActivity element) => TxType.Tx;

        public TxType Visit(ContractScriptActivity element) => TxType.NonTx;

        public TxType Visit(ContractTimerBoundaryEvent contractTimerBoundaryEvent) => TxType.Tx;
    }
}
