// This file is a part of MarkdownToPdf Library by Geert-Jan Thomas based on earlier work by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using VectorAi.MarkdownToPdf.Converters;

namespace VectorAi.MarkdownToPdf.Plugins;

// Any math or image plugin must use this interface. Recommended output is either PNG or PDF.
public interface IImagePlugin
{
    /// <summary>
    /// Main exposed function that should perform the conversion
    /// </summary>
    /// <param name="data">text of link description or a $math$ expression</param>
    /// <param name="converter">the converter calling the plugin</param>
    /// <returns></returns>
    ImagePluginResult Convert(string data, IElementConverter converter);
}
