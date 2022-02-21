using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process.Activities
{
    public class ContractScriptActivity : ContractActivityWithCode
    {
        public IEnumerable<string> TransitionCodeLines => ReadPragma(TransitionPragma);

        protected override bool IsCodePragma(string code)
        {
            code = code.Trim().ToUpperInvariant();

            return base.IsCodePragma(code) ||
                code == TransitionPragma.ToUpperInvariant();
        }


        const string TransitionPragma = "{-# TRANSITION #-}";

    }
}
