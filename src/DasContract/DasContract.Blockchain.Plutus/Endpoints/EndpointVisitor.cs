using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using DasContract.Blockchain.Plutus.Data.Processes.Process;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Activities;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Events;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Gateways;
using DasContract.Blockchain.Plutus.Data.Processes.Process.MultiInstances;

namespace DasContract.Blockchain.Plutus.Transitions
{
    public class EndpointVisitor : RecursiveElementVisitor
    {
        public EndpointVisitor(ContractStartEvent initialStartEvent)
        {
            InitialStartEvent = initialStartEvent;
        }

        Dictionary<string, bool> isFirstTxDictionary = new Dictionary<string, bool>();

        public ContractStartEvent InitialStartEvent { get; }

        bool HasFirstFlag(INamable element) => isFirstTxDictionary.ContainsKey(element.Name);

        void ThisHasNotFirstFlag(INamable element) => isFirstTxDictionary.Add(element.Name, false);

        void ThisHasFirstFlag(INamable element) => isFirstTxDictionary.Add(element.Name, true);

        void SendFlagFurther(INamable source, INamable target)
        {
            //Send flag further
            if (HasFirstFlag(source))
                ThisHasFirstFlag(target);
            else
                ThisHasNotFirstFlag(target);
        }

        void SendFlagFurther(INamable source, IEnumerable<INamable> targets)
        {
            foreach(var target in targets)
                SendFlagFurther(source, target);
        }

        bool NeedsEndpoint(ContractProcessElement element)
        {
            if (element is ContractUserActivity)
                return true;
            return false;
        }



        #region elementEndpoints
        public override IPlutusCode Visit(ContractExclusiveGateway element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            //Send flag further
            SendFlagFurther(element, element.Outgoing.Select(e => e.Target));

            //Evaluate further elements
            IPlutusCode result = PlutusCode.Empty;
            foreach (var target in element.Outgoing)
                result = result.Append(target.Target.Accept(this));

            return result;
        }

        public override IPlutusCode Visit(ContractMergingExclusiveGateway element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            //Send flag further
            SendFlagFurther(element, element.Outgoing);

            //Evaluate further elements
            return element.Outgoing.Accept(this);
        }

        public override IPlutusCode Visit(ContractStartEvent element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            //Check for initial start init
            if (InitialStartEvent == element)
                ThisHasFirstFlag(element);

            //Send flag further
            SendFlagFurther(element, element.Outgoing);

            //Evaluate further elements
            return element.Outgoing.Accept(this);
        }

        public override IPlutusCode Visit(ContractEndEvent element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            return PlutusCode.Empty;
        }

        public override IPlutusCode Visit(ContractCallActivity element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            //Send flag further
            SendFlagFurther(element, element.Outgoing);
            SendFlagFurther(element, element.CalledProcess.StartEvent);

            //Evaluate further elements
            return element.Outgoing.Accept(this)
                .Append(element.CalledProcess.StartEvent.Accept(this));
        }

        public override IPlutusCode Visit(ContractUserActivity element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            var timeoutBoundary = element.BoundaryEvents.OfType<ContractBoundaryEvent>().FirstOrDefault();

            //Is this first?
            if (HasFirstFlag(element))
            {
                //TODO first 

                ThisHasNotFirstFlag(element);
            }

            //TODO generate endpoint

            //Send flag further
            SendFlagFurther(element, element.Outgoing);
            if (!(timeoutBoundary is null))
                SendFlagFurther(element, timeoutBoundary);

            //Evaluate further elements
            IPlutusCode result = element.Outgoing.Accept(this);
            if (!(timeoutBoundary is null))
                result = result.Append(timeoutBoundary.Accept(this));
            return result
        }

        public override IPlutusCode Visit(ContractScriptActivity element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            //Send flag further
            SendFlagFurther(element, element.Outgoing);

            //Evaluate further elements
            return element.Outgoing.Accept(this);
        }

        public override IPlutusCode Visit(ContractTimerBoundaryEvent element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            //Send flag further
            SendFlagFurther(element, element.TimeOutDirection);

            //Evaluate further elements
            return element.TimeOutDirection.Accept(this);
        }
        #endregion
    }
}
