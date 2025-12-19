// This file is a part of MarkdownToPdf Library by Geert-Jan Thomas based on earlier work by Tomas Kubec
// Distributed under MIT license - see license.txt
//

namespace VectorAi.MarkdownToPdf.Styling;

/// <summary>
/// Descriptor for markdown element and all it's ancestors, containing their type, attributes and position in markdown document tree
/// </summary>

public class StylingDescriptor
{
    /// <summary>
    /// Descriptors for this element and each it's ancestor
    /// </summary>
    public readonly List<SingleElementDescriptor> Descriptors;

    /// <summary>
    /// Descriptor of current element
    /// </summary>
    public SingleElementDescriptor? CurrentElement { get => Descriptors.FirstOrDefault(); }

    public SingleElementDescriptor this[int key]
    {
        get => Descriptors[key];
    }

    internal StylingDescriptor(List<SingleElementDescriptor> descriptors)
    {
        this.Descriptors = descriptors ?? new List<SingleElementDescriptor>();
    }

    public bool HasParent(ElementType t, string styleName = "")
    {
        if (Descriptors.Count < 2) return false;
        return Descriptors[1].Type == t && (Descriptors[1].Attributes.Style == styleName || !!string.IsNullOrEmpty(styleName));
    }

    public bool HasAncestor(ElementType t, string styleName = "")
    {
        return Descriptors.Skip(1).Any(x => x.Type == t && (x.Attributes.Style == styleName || !!string.IsNullOrEmpty(styleName)));
    }

    public bool HasParentWithId(string id)
    {
        if (Descriptors.Count < 2) return false;
        return Descriptors[1].Attributes.Id == id;
    }

    public bool HasAncestorWithId(string id)
    {
        return Descriptors.Skip(1).Any(x => x.Attributes.Id == id);
    }
}
