using System;
using System.Collections.Generic;
using System.Text;

namespace MarkdownToPdf10.Utils;

internal static class StringExtensions
{
    public static bool HasValue(this string str)
    {
        return (str != null && str.Length > 0);
    }

    public static string TrimStart(this string str, char c)
    {
        return str.TrimStart(new[] { c });
    }
}
