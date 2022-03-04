﻿
using System;
using System.Collections.Generic;
using System.IO;
using DasContract.Blockchain.Plutus.Data.Forms;
using DasContract.Blockchain.Plutus.Data.Users;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process.Activities
{
    public class ContractUserActivity : ContractActivityWithCode
    {
        public ContractForm Form { get; set; } = new ContractForm();

        public ContractUser? Assignee { get; set; }

        public string AssigneeId { get; set; } = string.Empty;

        public ICollection<ContractUser> CandidateUsers { get; set; } = new List<ContractUser>();

        public ICollection<string> CandidateUserIds { get; set; } = new List<string>();

        public ICollection<ContractRole> CandidateRoles { get; set; } = new List<ContractRole>();

        public ICollection<string> CandidateRoleIds { get; set; } = new List<string>();

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


        public const string FormValidationPragma = "{-# FORM_VALIDATION #-}";

        public const string ExpectedValuePragma = "{-# EXPECTED_VALUE #-}";

        public const string NewValuePragma = "{-# NEW_VALUE #-}";

        public const string ConstrainsPragma = "{-# CONSTRAINS #-}";

        public const string TransitionPragma = "{-# TRANSITION #-}";

        /// <inheritdoc/>
        public override T Accept<T>(IContractProcessElementVisitor<T> visitor)
            => visitor.Visit(this);
    }
}
