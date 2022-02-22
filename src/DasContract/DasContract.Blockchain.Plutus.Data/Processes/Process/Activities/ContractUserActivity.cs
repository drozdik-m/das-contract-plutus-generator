
using System;
using System.Collections.Generic;
using System.IO;
using DasContract.Blockchain.Plutus.Data.Forms;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process.Activities
{
    public class ContractUserActivity : ContractActivityWithCode
    {
        public ContractForm Form { get; set; } = new ContractForm();

        public string FormName => Name + "Form";


        public IEnumerable<string> FormValidationCodeLines => ReadPragma(FormValidationPragma);

        public IEnumerable<string> ExpectedValueCodeLines => ReadPragma(ExpectedValuePragma);

        public IEnumerable<string> NewValueCodeLines => ReadPragma(NewValuePragma);

        public IEnumerable<string> ContrainsCodeLines => ReadPragma(ConstrainsPragma);

        public IEnumerable<string> TransitionCodeLines => ReadPragma(TransitionPragma);

        protected override bool IsCodePragma(string code)
        {
            code = code.Trim().ToUpperInvariant();

            return base.IsCodePragma(code) ||
                code == FormValidationPragma.ToUpperInvariant() ||
                code == ExpectedValuePragma.ToUpperInvariant() ||
                code == NewValuePragma.ToUpperInvariant() ||
                code == ConstrainsPragma.ToUpperInvariant() ||
                code == TransitionPragma.ToUpperInvariant();
        }


        const string FormValidationPragma = "{-# FORM_VALIDATION #-}";

        const string ExpectedValuePragma = "{-# EXPECTED_VALUE #-}";

        const string NewValuePragma = "{-# NEW_VALUE #-}";

        const string ConstrainsPragma = "{-# CONSTRAINS #-}";

        const string TransitionPragma = "{-# TRANSITION #-}";

        /// <inheritdoc/>
        public override T Accept<T>(IContractProcessElementVisitor<T> visitor)
            => visitor.Visit(this);
    }
}
