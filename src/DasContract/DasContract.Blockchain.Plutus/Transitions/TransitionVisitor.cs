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
    public abstract class TransitionVisitor : IContractProcessElementVisitor<IPlutusCode>
    {
        public TransitionVisitor()
        {
            
        }

        public TransitionVisitor(INamable subprocess)
            :this()
        {
            Subprocess = subprocess;
        }

        public INamable? Subprocess { get; } = null;

        protected HashSet<string> VisitedElements { get; } = new HashSet<string>();

        protected string AddSubprocessPrefix(INamable? subprocess, string current)
        {
            if (!(subprocess is null))
            {
                if (current.Any(char.IsWhiteSpace))
                    current = $"{subprocess.Name} ({current})";
                else
                    current = $"{subprocess.Name} {current}";
            }

            return current;
        }

        protected string CurrentElementName(ContractProcessElement element, INamable? subprocess = null)
        {
            var result = element.Name;

            //Check for sequential loop
            if (element is ContractActivity activity
                && activity.MultiInstance is ContractSequentialMultiInstance)
            {
                result = $"{result} LoopEnded";
            }

            if (subprocess is null)
                return result;
            return AddSubprocessPrefix(subprocess, result);
        }

        protected string FutureElementName(ContractProcessElement element, INamable? subprocess = null)
        {
            var result = element.Name;

            //Check for sequential loop
            if (element is ContractActivity activity
                && activity.MultiInstance is ContractSequentialMultiInstance sequentialMultiInstance)
            {
                var toLoop = sequentialMultiInstance.LoopCardinality;
                if (toLoop.Any(char.IsWhiteSpace))
                    toLoop = $"$ {toLoop}";
                result = $"{result} (toSeqMultiInstance {toLoop})";
            }

            if (subprocess is null)
                return result;
            return AddSubprocessPrefix(subprocess, result);
        }

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

        /// <summary>
        /// Returns a comment that indicates transition direction
        /// </summary>
        /// <param name="sourceName">The name of the source</param>
        /// <param name="targetName">The name of the target</param>
        /// <returns></returns>
        protected IPlutusCode TransitionComment(int indent, string sourceName, string targetName)
        {
            return new PlutusCode(new IPlutusLine[]
            {
                new PlutusComment(indent, $"{sourceName} -> {targetName}")
            });
        }

        /// <summary>
        /// Returns a comment that indicates transition direction with a stack-return note
        /// </summary>
        /// <param name="sourceName">The name of the source</param>
        /// <param name="targetName">The name of the target</param>
        /// <returns></returns>
        protected IPlutusCode TransitionCommentWithReturn(int indent, string sourceName, string targetName, string returnName)
        {
            return new PlutusCode(new IPlutusLine[]
            {
                new PlutusComment(indent, $"{sourceName} -> {targetName} / return {returnName}")
            });
        }

        /// <summary>
        /// Returns a comment that indicates transition direction with a timeout note
        /// </summary>
        /// <param name="sourceName">The name of the source</param>
        /// <param name="targetName">The name of the target</param>
        /// <returns></returns>
        protected IPlutusCode TransitionCommentWithTimeout(int indent, string sourceName, string targetName)
        {
            return new PlutusCode(new IPlutusLine[]
            {
                new PlutusComment(indent, $"{sourceName} -> {targetName} /timeout/")
            });
        }


        public abstract IPlutusCode Visit(ContractExclusiveGateway element);
        public abstract IPlutusCode Visit(ContractMergingExclusiveGateway element);
        public abstract IPlutusCode Visit(ContractCallActivity element);
        public abstract IPlutusCode Visit(ContractUserActivity element);
        public abstract IPlutusCode Visit(ContractScriptActivity element);
        public abstract IPlutusCode Visit(ContractTimerBoundaryEvent contractTimerBoundaryEvent);
        public abstract IPlutusCode Visit(ContractStartEvent element);
        public abstract IPlutusCode Visit(ContractEndEvent element);
    }
}
