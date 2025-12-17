// This file is a part of MarkdownToPdf Library by Geert-Jan Thomas based on earlier work by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using VectorAi.MarkdownToPdf.Converters;
using VectorAi.MarkdownToPdf.Plugins;
using VectorAi.MarkdownToPdf.Utils;

namespace VectorAi.MarkdownToPdf;

/// <summary>
/// Enables adding plugins
/// </summary>

public sealed class PluginManager
{
    private readonly List<IHighlightingPlugin> _highlightingPlugins = new List<IHighlightingPlugin>();
    private readonly List<IImagePlugin> _imagePlugins = new List<IImagePlugin>();
    private readonly MarkdownToPdf _owner;

    internal PluginManager(MarkdownToPdf owner)
    {
        this._owner = owner;
    }

    /// <summary>
    /// Adds highlighting plugin. If more plugins are added and the first is unable to higlight the data, the following plugins get a chance
    /// </summary>
    public void Add(IHighlightingPlugin highlightingPlugin)
    {
        _highlightingPlugins.Add(highlightingPlugin);
    }

    /// <summary>
    /// Adds a math or image  plugin. If more plugins are added and the first is unable to convertt the data, the following plugins get a chance
    /// </summary>
    public void Add(IImagePlugin imagePlugin)
    {
        _imagePlugins.Add(imagePlugin);
    }

    /// <summary>
    /// Adds a math plugin and enables $math$ parsing. If more plugins are added and the first is unable to convertt the data, the following plugins get a chance
    /// </summary>
    public void AddMathPlugin(IImagePlugin imagePlugin)
    {
        _imagePlugins.Add(imagePlugin);
        _owner.ConversionSettings.UseMath();
    }

    internal HighlightingPluginResult? Highlight(List<string> lines, IElementConverter converter)
    {
        foreach (var p in _highlightingPlugins)
        {
            try
            {
                var res = p.Convert(lines, converter);
                if (res.Success) return res;

                if (res.Message != null) _owner.OnWarningIssued(this, "HighlightPlugin", res.Message);
            }
            catch (Exception e)
            {
                _owner.OnWarningIssued(this, "Plugin", e.Message);
            }
        }
        return null;
    }

    internal ImagePluginResult? GetImage(string data, IElementConverter converter)
    {
        foreach (var p in _imagePlugins)
        {
            try
            {
                var res = p.Convert(data, converter);
                if (res.Success)
                {
                    _owner.tempFiles.Add(res.FileName);
                    return res;
                }

                if (res.Message != null) _owner.OnWarningIssued(this, "ImagePlugin", res.Message);
            }
            catch (Exception e)
            {
                _owner.OnWarningIssued(this, "Plugin", e.Message);
            }
        }
        return null;
    }
}
