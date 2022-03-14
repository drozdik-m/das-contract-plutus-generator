using System;

namespace DasContract.String.Utils
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns new string with the first letter as lower
        /// Source (https://stackoverflow.com/questions/21755757/first-character-of-string-lowercase-c-sharp)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FirstCharToLowerCase(this string str)
        {
            if (string.IsNullOrEmpty(str) || char.IsLower(str[0]))
                return str;

            return char.ToLower(str[0]) + str[1..];
        }

        /// <summary>
        /// Returns new string with the first letter as upper
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FirstCharToUpperCase(this string str)
        {
            if (string.IsNullOrEmpty(str) || char.IsLower(str[0]))
                return str;

            return char.ToUpper(str[0]) + str[1..];
        }
    }
}
