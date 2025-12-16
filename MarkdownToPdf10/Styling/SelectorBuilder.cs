using MarkdownToPdf10.Styling.Style;

namespace MarkdownToPdf10.Styling;

/// <summary>
/// Fluent binding of styles
/// </summary>
public class SelectorBuilder
{
    private readonly StyleManager _styleManager;

    private readonly List<StyleSelector> _selectors;

    internal SelectorBuilder(StyleManager styleManager, ElementType type, string styleName)
    {
        this._styleManager = styleManager;
        _selectors = new List<StyleSelector>
            {
                new StyleSelector { ElementType = type, StyleName = styleName }
            };
    }

    /// <summary>
    /// Declares that the style is used only if previous selector in chain has parent of specified type (and name)
    /// </summary>
    public SelectorBuilder WithParent(ElementType type, string styleName = "")
    {
        _selectors.Add(new StyleSelector { ElementType = type, StyleName = styleName, SelectorType = StyleSelector.SelectorTypes.Parent });
        return this;
    }

    /// <summary>
    /// Declares that the style is used only if previous selector in chain has ancestor of specified type (and name)
    /// </summary>
    public SelectorBuilder WithAncestor(ElementType type, string styleName = "")
    {
        _selectors.Add(new StyleSelector { ElementType = type, StyleName = styleName, SelectorType = StyleSelector.SelectorTypes.Ancestor });
        return this;
    }

    /// <summary>
    /// Declares that the style is used only if the filter condition is true. Usefull for advanced styling.
    /// </summary>
    /// <param name="filter">function deciding whether the style matches according to the passed StylingDescriptor</param>
    /// <returns></returns>
    public SelectorBuilder Where(Func<StylingDescriptor, bool> filter)
    {
        _selectors.Add(new StyleSelector { SelectorType = StyleSelector.SelectorTypes.Filter, Filter = filter });
        return this;
    }

    /// <summary>
    /// Binds the style to markdown elements according to conditions based on preceeding selector chain.
    /// </summary>
    public void Bind(CascadingStyle style)
    {
        _styleManager.Bind(_selectors, (style, null));
    }

    /// <inheritdoc cref="Bind" />
    public void Bind(string styleName)
    {
        Bind(_styleManager.Styles[styleName]);
    }

    /// <summary>
    /// Binds the style to markdown elements according to conditions based on preceeding selector chain. When the style is evaluated, the modification method is called and can adjust the evaluated style
    /// </summary>
    public void BindAndModify(CascadingStyle style, Action<CascadingStyle, StylingDescriptor> modificationMethod)
    {
        _styleManager.Bind(_selectors, (style, modificationMethod));
    }

    /// <inheritdoc cref="BindAndModify" />
    public void BindAndModify(string styleName, Action<CascadingStyle, StylingDescriptor> modificationMethod)
    {
        BindAndModify(_styleManager.Styles[styleName], modificationMethod);
    }
}
