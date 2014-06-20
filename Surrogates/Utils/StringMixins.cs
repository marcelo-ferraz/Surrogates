using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Surrogates.Utils
{
    public static class StringMixins
    {
        private static Regex _fieldNameDonts;

        static StringMixins()
        {
            _fieldNameDonts = new Regex(
                "(^[^A-Za-z])|([^A-Za-z0-9_])", RegexOptions.Compiled);
        }

        public static bool CanBeFieldName(this string self)
        {
            return _fieldNameDonts.IsMatch(self);
        }
    }
}
