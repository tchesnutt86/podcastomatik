using System;
using System.Collections.Generic;
using System.Text;

namespace Podcastomatik.Shared.Extensions
{
    public static class UtilityExtensions
    {
        public static bool CaseInsensitiveContains(this string text, string value,
            StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
        {
            return text.IndexOf(value, stringComparison) >= 0;
        }
    }
}
