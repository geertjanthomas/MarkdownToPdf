// This file is a part of MarkdownToPdf Library by Geert-Jan Thomas based on earlier work by Tomas Kubec
// Distributed under MIT license - see license.txt
//

namespace VectorAi.MarkdownToPdf.Utils;

internal static class StringExtensions
{
    public static string TrimStart(this string str, char c)
    {
        return str.TrimStart(new[] { c });
    }
}
