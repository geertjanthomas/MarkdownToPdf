// This file is a part of MarkdownToPdf Library by Geert-Jan Thomas based on earlier work by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Markdig.Syntax;
using VectorAi.MarkdownToPdf.MigrDoc;
using VectorAi.MarkdownToPdf.Styling;
using System.Text.RegularExpressions;

namespace VectorAi.MarkdownToPdf.Converters.ContainerConverters;

internal class RootBlockConvertor : ContainerBlockConverter, IStandaloneContainerConverter
{
    public RootBlockConvertor(MarkdownDocument block, string rawText, MigraDocBlockContainer output,
        ContainerBlockConverter? parent, MarkdownToPdf owner)
         : base(block, parent)
    {
        Owner = owner;
        RawText = rawText;
        OutputContainer = output;
        ElementDescriptor = new SingleElementDescriptor { Attributes = Attributes, Type = ElementType.Root };
        Width = Owner.RealPageWidth.Point;
        Split(rawText);
    }

    private void Split(string rawText)
    {
        var matches = Regex.Matches(rawText, "^.*(\r\n|\r|\n)", RegexOptions.Multiline);
        Lines = matches.Cast<Match>().Select(match => match.Value).ToList();
    }

    protected override void PrepareStyling()
    {
        base.PrepareStyling();
        Width = Owner?.RealPageWidth.Point ?? 0;
        FontSize = EvaluatedStyle.Font.Size.Eval(12, Owner?.RealPageWidth.Point ?? 0).Point;
    }
}
