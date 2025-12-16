using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace MarkdownToPdf10.Utils;

internal static class MarkdigExtensions
{
    public static bool IsFirst(this Block block)
    {
        return (block.Parent?.IndexOf(block) ?? -1) == 0;
    }

    public static bool IsFirst(this Inline inline)
    {
        return (inline.Parent?.FirstChild) == inline;
    }

    public static bool IsLast(this Block block)
    {
        if (block.Parent == null) return false;
        return block.Parent.IndexOf(block) == block.Parent.Count - 1;
    }

    public static bool IsLast(this Inline inline)
    {
        return (inline.Parent?.LastChild) == inline;
    }

    public static int GetIndex(this Inline inline)
    {
        if (inline.Parent == null) return -1;
        return inline.Parent.ToList().IndexOf(inline);
    }

    public static int GetIndex(this Block block)
    {
        return block.Parent?.IndexOf(block) ?? -1;
    }
}
