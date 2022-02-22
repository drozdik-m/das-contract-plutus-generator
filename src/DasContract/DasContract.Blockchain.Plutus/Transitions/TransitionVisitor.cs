using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using DasContract.Blockchain.Plutus.Data.Processes.Process;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Activities;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Events;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Gateways;
using DasContract.Blockchain.Plutus.Data.Processes.Process.MultiInstances;

namespace DasContract.Blockchain.Plutus.Transitions
{
    public abstract class TransitionVisitor : IContractProcessElementVisitor<IPlutusCode>
    {
        public TransitionVisitor(ContractProcessElement sourceElement)
        {
            SourceElement = sourceElement;
        }

        public TransitionVisitor(ContractProcessElement sourceElement, INamable subprocess)
            :this(sourceElement)
        {
            Subprocess = subprocess;
        }

        public INamable? Subprocess { get; }

        protected ContractProcessElement SourceElement { get; }

        protected string AddSubprocessPrefix(string current)
        {
            if (!(Subprocess is null))
            {
                if (current.Any(char.IsWhiteSpace))
                    current = $"{Subprocess.Name} ({current})";
                else
                    current = $"{Subprocess.Name} ({current})";
            }

            return current;
        }

        protected string CurrentElementName
        {
            get
            {
                var result = SourceElement.Name;

                //Check for sequential loop
                if (SourceElement is ContractActivity activity 
                    && activity.MultiInstance is ContractSequentialMultiInstance)
                {
                    result = $"{result} LoopEnded";
                }

                return AddSubprocessPrefix(result);
            }
        }

        protected string FutureElementName(ContractProcessElement element);

        protected string FutureElementName(ContractActivity activity);

        public IPlutusCode Visit(ContractStartEvent element)
        {
            throw new Exception("Start event can not process an input");
        }

        public abstract IPlutusCode Visit(ContractExclusiveGateway element);
        public abstract IPlutusCode Visit(ContractMergingExclusiveGateway element);
        public abstract IPlutusCode Visit(ContractEndEvent element);
        public abstract IPlutusCode Visit(ContractCallActivity element);
        public abstract IPlutusCode Visit(ContractUserActivity element);
        public abstract IPlutusCode Visit(ContractScriptActivity element);
        public abstract IPlutusCode Visit(ContractTimerBoundaryEvent contractTimerBoundaryEvent);
    }
}
