using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Blockchain.Plutus.Code.Comments;
using DasContract.Blockchain.Plutus.Code.Types.Determined;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using DasContract.Blockchain.Plutus.Data.Processes.Process;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Activities;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Events;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Gateways;

namespace DasContract.Blockchain.Plutus.Transitions.NoTx
{
    public abstract class NoTxTransitionVisitor : TransitionVisitor
    {
        public NoTxTransitionVisitor(ContractProcessElement sourceElement)
            : base(sourceElement)
        {
           
        }

        public NoTxTransitionVisitor(ContractProcessElement sourceElement, INamable subprocess)
            : base(sourceElement, subprocess)
        {
            
        }



        public static PlutusFunctionSignature TransitionFunctionSignature { get; }
            = new PlutusFunctionSignature(0, "doNoTxTransition", new INamable[]
            {
                PlutusContractDatum.Type,
                PlutusContractDatum.Type,
            });
    }
}
