using System.Linq.Expressions;

namespace arx.Extract.Lib
{
    public static class StringUtils
    {
        /// <summary>
        /// Returns the string after the specified characters value. 
        /// Returns an empty string, if the string is empty, the char
        /// value doesn't exist in the string.
        /// </summary>
        /// <param name="input">String</param>
        /// <param name="value">char</param>
        /// <returns>string</returns>
        public static string GetSubStringAfterCharValue(this string input, char value, bool includeCharValue = false)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            int index = input.LastIndexOf(value);

            if (index == -1 || index >= input.Length)
                return string.Empty;

            int startIndex = includeCharValue ? index : index + 1;
            
            return input.Substring(startIndex);
        }
    }
}
