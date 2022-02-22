using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using DasContract.Blockchain.Plutus.Data.Processes.Process;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Activities;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Events;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Gateways;
using System.Linq;

namespace DasContract.Blockchain.Plutus.Transitions.NoTx
{
    public class NoTxTrivialTransitionVisitor : NoTxTransitionVisitor
    {
        public NoTxTrivialTransitionVisitor(ContractProcessElement sourceElement)
            : base(sourceElement)
        {
           
        }

        public NoTxTrivialTransitionVisitor(ContractProcessElement sourceElement, INamable subprocess)
            : base(sourceElement, subprocess)
        {
            
        }


        public override IPlutusCode Visit(ContractExclusiveGateway element)
        {
            throw new NotImplementedException();
        }

        public override IPlutusCode Visit(ContractMergingExclusiveGateway element)
        {

            //var transitionFunction = new PlutusFunction(0, scriptTransitionSig, );

            var transitionComment = new PlutusComment(0, $"{CurrentElementName} -> {element.Name}");
        }

        public override IPlutusCode Visit(ContractEndEvent element)
        {
            throw new NotImplementedException();
        }

        public override IPlutusCode Visit(ContractCallActivity element)
        {
            throw new NotImplementedException();
        }

        public override IPlutusCode Visit(ContractUserActivity element)
        {
            throw new NotImplementedException();
        }

        public override IPlutusCode Visit(ContractScriptActivity element)
        {
            throw new NotImplementedException();
        }

        public override IPlutusCode Visit(ContractTimerBoundaryEvent contractTimerBoundaryEvent)
        {
            throw new NotImplementedException();
        }
    }
}
