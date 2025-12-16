using MarkdownToPdf10.Styling.Style;
using MarkdownToPdf10.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarkdownToPdf10.Styling;

/// <summary>
/// <b>Class to access, create and register styles used during conversion</b>
/// </summary>
public class StyleManager
{
    private Dictionary<List<StyleSelector>, (CascadingStyle Style, Action<CascadingStyle, StylingDescriptor> ModificationMethod)> _bindings;
    private readonly Dictionary<string, CascadingStyle> _styles;

    /// <summary>
    /// The main converter class
    /// </summary>
    public MarkdownToPdf Owner { get; private set; }

    /// <summary>
    /// All existing styles
    /// </summary>
    public IReadOnlyDictionary<string, CascadingStyle> Styles { get => _styles; }

    internal StyleManager(MarkdownToPdf owner)
    {
        this.Owner = owner;
        _bindings = new Dictionary<List<StyleSelector>, (CascadingStyle Style, Action<CascadingStyle, StylingDescriptor> ModificationMethod)>();

        _styles = new Dictionary<string, CascadingStyle>();
    }

    /// <summary>
    /// Creats and add new style
    /// </summary>
    /// <param name="name">optional style name for easier later access</param>
    /// <param name="baseStyle">Base style. Current style inherits all ancestors properties and adds/redefines new ones. It is not a copy, so later changes to ancestors are reflected.</param>
    public CascadingStyle AddStyle(string name, CascadingStyle baseStyle = null)
    {
        name = name.HasValue() ? name : Guid.NewGuid().ToString();
        var newStyle = new CascadingStyle(name, baseStyle);

        if (_styles.ContainsKey(name))
        {
            var oldStyle = _styles[name];
            _styles[name] = newStyle;

            var oldBindings = _bindings.Where(x => x.Value.Style == oldStyle).ToDictionary(x => x.Key, x => x.Value).ToList();

            _bindings = _bindings.Where(x => x.Value.Style != oldStyle).ToDictionary(x => x.Key, x => x.Value);

            foreach (var b in oldBindings)
            {
                _bindings.Add(b.Key, (newStyle, b.Value.ModificationMethod));
            }
        }
        else
        {
            _styles.Add(name, newStyle);
        }

        return newStyle;
    }

    /// <summary>
    /// Creats and add new style
    /// </summary>
    /// <param name="name">optional style name for easier later access</param>
    /// <param name="baseStyle">Base style name (see also <seealso cref="MarkdownStyleNames"/>). Current style inherits all ancestors properties and adds/redefines new ones. It is not a copy, so later changes to ancestors are reflected.</param>
    public CascadingStyle AddStyle(string name, string baseStyle)
    {
        return AddStyle(name, _styles[baseStyle]);
    }

    /// <summary>
    /// Premise for style binding, defining for which elemnt type the binding will be used
    /// </summary>
    /// <param name="element">type of element</param>
    /// <param name="style">optional style name</param>
    /// <returns></returns>
    public SelectorBuilder ForElement(ElementType element, string style = "")
    {
        return new SelectorBuilder(this, element, style);
    }

    internal CascadingStyle GetStyle(StylingDescriptor descriptor)
    {
        var matchingBindings = MatchBindnigs(_bindings, descriptor);
        var style = GetBestMatchingSyle(matchingBindings);
        if (style == default)
        {
            return Owner.StyleManager._styles[MarkdownStyleNames.Undefined];
        }
        var res = style.Style.Clone();
        style.ModificationMethod?.Invoke(res, descriptor);
        return res;
    }

    internal void Bind(List<StyleSelector> selectors, (CascadingStyle Style, Action<CascadingStyle, StylingDescriptor> ModificationMethod) style)
    {
        // find if the same selector exists:
        var key = _bindings.Keys.FirstOrDefault(x => StyleSelector.IsEqualSelectorList(x, selectors));

        if (key == null)
        {
            _bindings.Add(selectors, style);
        }
        else
        {
            _bindings[key] = style;
        }
    }

    private Dictionary<List<StyleSelector>, (CascadingStyle Style, Action<CascadingStyle, StylingDescriptor> ModificationMethod)> MatchBindnigs(Dictionary<List<StyleSelector>, (CascadingStyle Style, Action<CascadingStyle, StylingDescriptor> ModificationMethod)> bindings,
                       StylingDescriptor descriptor, int level = 0, Dictionary<List<StyleSelector>, int> progress = null)
    {
        if (!bindings.Any()) return bindings;

        var maxLevels = bindings.Keys.Select(x => x.Count).Max();
        if (level > maxLevels) return bindings;

        progress = progress ?? bindings
            .Select(xx => new KeyValuePair<List<StyleSelector>, int>(xx.Key, 0))
            .ToDictionary(x => x.Key, x => x.Value);

        var res = bindings.Where(binding => BindingFilter(descriptor, level, progress, binding)).ToList();

        return MatchBindnigs(res.ToDictionary(x => x.Key, x => x.Value), descriptor, ++level, progress);
    }

    private (CascadingStyle Style, Action<CascadingStyle, StylingDescriptor> ModificationMethod) GetBestMatchingSyle(Dictionary<List<StyleSelector>, (CascadingStyle Style, Action<CascadingStyle, StylingDescriptor> ModificationMethod)> bindings, int level = 0)
    {
        if (!bindings.Any()) return default;
        if (bindings.Count == 1) return bindings.First().Value;

        // with Name And Type
        var res = bindings.Where(x => x.Key.Count > level && x.Key[level].StyleName.HasValue() && x.Key[level].ElementType != ElementType.Any);
        var preferred = res.Where(x => x.Key.Count > level && x.Key[level].SelectorType == StyleSelector.SelectorTypes.Parent);
        if (preferred.Any()) res = preferred;

        // with name
        if (!res.Any())
        {
            res = bindings.Where(x => x.Key.Count > level && x.Key[level].StyleName.HasValue());
            preferred = res.Where(x => x.Key.Count > level && x.Key[level].SelectorType == StyleSelector.SelectorTypes.Parent);
            if (preferred.Any()) res = preferred;
        }

        if (!res.Any())
        {
            res = bindings.Where(x => x.Key.Count > level);
            preferred = res.Where(x => x.Key.Count > level && x.Key[level].SelectorType == StyleSelector.SelectorTypes.Parent);
            if (preferred.Any()) res = preferred;
        }

        if (res.Count() > 1)
        {
            res = res.ToList();
            level++;
            return GetBestMatchingSyle(res.ToDictionary(x => x.Key, x => x.Value), level);
        }

        if (!res.Any()) return bindings.First().Value;

        return res.First().Value;
    }

    private static bool BindingFilter(StylingDescriptor descriptor, int level, Dictionary<List<StyleSelector>, int> descriptorPositions,
        KeyValuePair<List<StyleSelector>, (CascadingStyle Style, Action<CascadingStyle, StylingDescriptor> ModificationMethod)> binding)
    {
        var descriptors = descriptor.Descriptors;
        if (binding.Key.Count <= level) return true;

        if (descriptors.Count <= descriptorPositions[binding.Key] && binding.Key[level].SelectorType != StyleSelector.SelectorTypes.Filter) return false;

        var current = binding.Key[level].SelectorType != StyleSelector.SelectorTypes.Filter ? descriptors[descriptorPositions[binding.Key]] : descriptors[0];

        var bidningSelector = binding.Key[level];

        switch (bidningSelector.SelectorType)
        {
            case StyleSelector.SelectorTypes.Filter:
                {
                    if (!bidningSelector?.Filter(descriptor) ?? true)
                    {
                        return false;
                    }
                    descriptorPositions[binding.Key]++;
                    return true;
                }
            case StyleSelector.SelectorTypes.Base:
                {
                    if (bidningSelector.ElementType != descriptors.First().Type)
                    {
                        return false;
                    }
                    if (bidningSelector.StyleName.HasValue() && bidningSelector.StyleName != descriptors.First().Attributes.Style)
                    {
                        return false;
                    }
                    descriptorPositions[binding.Key]++;
                    return true;
                }

            case StyleSelector.SelectorTypes.Parent:
                {
                    if (bidningSelector.ElementType != ElementType.Any && current.Type != bidningSelector.ElementType)
                    {
                        return false;
                    }
                    if (bidningSelector.StyleName.HasValue() && bidningSelector.StyleName != current.Attributes.Style)
                    {
                        return false;
                    }
                    descriptorPositions[binding.Key]++;
                    return true;
                }

            case StyleSelector.SelectorTypes.Ancestor:
                {
                    if (descriptors.Count < 2)
                    {
                        return false;
                    }

                    var anc = descriptors
                      .Skip(descriptorPositions[binding.Key])
                      .SkipWhile(xx => xx.Type != ElementType.Any && xx.Type != bidningSelector.ElementType
                        || (bidningSelector.StyleName.HasValue() && bidningSelector.StyleName != xx.Attributes.Style));

                    if (!anc.Any()) return false;
                    descriptorPositions[binding.Key] += descriptors.Count - anc.Count();
                    return true;
                }

            default:
                return false;
        }
    }
}
