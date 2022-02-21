using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process.Activities
{
    public class ContractActivityWithCode : ContractActivity
    {
        string Code { get; set; } = string.Empty;

        protected virtual IEnumerable<string> ReadPragma(string pragma)
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
                    if (IsCodePragma(line))
                        break;

                    result.Add(line);
                }

                //Looking for the pragma
                else if (line.Trim().ToUpperInvariant() == pragma.Trim().ToUpperInvariant())
                    readingPragma = true;
            }

            return result;
        }

        protected virtual bool IsCodePragma(string code) => false;
    }
}
