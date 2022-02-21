
using System;
using System.Collections.Generic;
using System.IO;
using DasContract.Blockchain.Plutus.Data.Forms;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process.Activities
{
    public class ContractUserActivity : ContractActivity
    {
        public string Code { get; set; } = string.Empty;

        public ContractForm Form { get; set; } = new ContractForm();

        public string FormName => Name + "Form";


        public IEnumerable<string> FormValidationCodeLines => ReadPragma(formValidationPragma);


        IEnumerable<string> ReadPragma(string pragma)
        {
            using var reader = new StringReader(Code);
            bool readingPragma = false;
            List<string> result = new List<string>();

            for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
            {
                //Read the code
                if (readingPragma)
                {
                    //Should stop reading the code
                    if (IsUserActivityPragma(line))
                        break;

                    result.Add(line);
                }

                //Looking for the pragma
                else if (line.Trim().ToUpperInvariant() == pragma.Trim().ToUpperInvariant())
                    readingPragma = true;
            }

            return result;
        }

        static bool IsUserActivityPragma(string code)
        {
            code = code.Trim().ToUpperInvariant();

            return
                code == formValidationPragma.ToUpperInvariant() ||
                code == expectedValuePragma.ToUpperInvariant() ||
                code == newValuePragma.ToUpperInvariant() ||
                code == constrainsPragma.ToUpperInvariant() ||
                code == transitionPragma.ToUpperInvariant();
        }

        const string formValidationPragma = "{-# FORM_VALIDATION #-}";

        const string expectedValuePragma = "{-# EXPECTED_VALUE #-}";

        const string newValuePragma = "{-# NEW_VALUE #-}";

        const string constrainsPragma = "{-# CONSTRAINS #-}";

        const string transitionPragma = "{-# TRANSITION #-}";
    }
}
