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
    /// <summary>
    /// A visitor that generates transitions
    /// </summary>
    public abstract class TransitionVisitor : RecursiveElementVisitor
    {
        public TransitionVisitor()
        {
            
        }

        /// <summary>
        /// The visitor is visiting a subprocess
        /// </summary>
        /// <param name="subprocess"></param>
        public TransitionVisitor(INamable subprocess)
            :this()
        {
            Subprocess = subprocess;
        }

        /// <summary>
        /// The visited subprocess.
        /// Null if the visitor is visiting the main process.
        /// </summary>
        public INamable? Subprocess { get; } = null;

        /// <summary>
        /// Adds subprocess-name prefix to a state (is means that the state is a state of the subprocess)
        /// </summary>
        /// <param name="subprocess"></param>
        /// <param name="current"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns element name as if the element is the source of a transition
        /// </summary>
        /// <param name="element"></param>
        /// <param name="subprocess"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns element name as if the element is the target of a transition
        /// </summary>
        /// <param name="element"></param>
        /// <param name="subprocess"></param>
        /// <returns></returns>
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

    }
}
