using System;
using System.Linq;

namespace Poprey.Core.Util
{
    public static class StringExtensions
    {
        public static string Capitalize(this string str)
        {
            switch (str)
            {
                case null: throw new ArgumentNullException(nameof(str));
                case "": throw new ArgumentException($"{nameof(str)} cannot be empty", nameof(str));
                default: return str.First().ToString().ToUpper() + str.Substring(1);
            }
        }

        public static string LowercaseAndUnderline(this string str)
        {
            switch (str)
            {
                case null: throw new ArgumentNullException(nameof(str));
                default: return str.ToLower().Replace(" ", "_");
            }
        }
    }
}