using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DasContract.Blockchain.Plutus.Console
{
    public class ConsoleArguments
    {
        private readonly List<string> arguments;

        public ConsoleArguments(IEnumerable<string> arguments)
        {
            this.arguments = arguments.ToList();
        }

        /// <summary>
        /// Returns count of the arguments
        /// </summary>
        public int Count => arguments.Count;  

        /// <summary>
        /// Checks if the string is a flag
        /// </summary>
        /// <param name="flagName"></param>
        /// <returns></returns>
        static bool IsFlag(string flagName) => flagName.StartsWith("-");

        /// <summary>
        /// Tries to get values for an argument flag
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool TryGetArgumentValues(string flag, out List<string> values)
        {
            values = new List<string>();    
            var reading = false;
            var result = false;
            foreach(var argument in arguments)
            { 
                //Start reading
                if (argument == flag)
                {
                    reading = true;
                    result = true;
                }

                //Already reading
                else if (reading)
                {
                    //Next argument encountered
                    if (IsFlag(argument))
                        reading = false;

                    //Still reading the argument values
                    else
                        values.Add(argument);
                }
            }

            return result;
        }

        /// <summary>
        /// Tries to get value for an argument flag
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool TryGetArgumentValue(string flag, out string value)
        {
            if (TryGetArgumentValues(flag, out var values))
            {
                if (values.Count != 1)
                    throw new Exception($"Expected a value for the flag {flag}");

                value = values.Single();
                return true;
            }

            value = string.Empty;
            return false;
        }

        /// <summary>
        /// Checks if a flag is defined
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public bool FlagExists(string flag)
        {
            return TryGetArgumentValues(flag, out var _);
        }
    }

}
