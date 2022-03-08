using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process.Activities
{
    public class ContractScriptActivity : ContractActivityWithCode
    {
        /// <summary>
        /// Code lines for transition
        /// </summary>
        public IEnumerable<string> TransitionCodeLines => ReadPragma(TransitionPragma);

        /// <inheritdoc/>
        protected override bool IsCodePragma(string code)
        {
            code = code.Trim().ToUpperInvariant();

            return base.IsCodePragma(code) ||
                code == TransitionPragma.ToUpperInvariant();
        }

        public const string TransitionPragma = "{-# TRANSITION #-}";

        /// <inheritdoc/>
        public override T Accept<T>(IContractProcessElementVisitor<T> visitor)
            => visitor.Visit(this);

    }
}
