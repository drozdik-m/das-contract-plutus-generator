using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process.Activities
{
    public abstract class ContractActivityWithCode : ContractActivity
    {
        /// <summary>
        /// The code that this activity contains
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Reads and returns code for a pragma
        /// </summary>
        /// <param name="pragma">The pragma</param>
        /// <returns></returns>
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

        /// <summary>
        /// Checks if a code line a pragma
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        protected virtual bool IsCodePragma(string code) => false;
    }
}
