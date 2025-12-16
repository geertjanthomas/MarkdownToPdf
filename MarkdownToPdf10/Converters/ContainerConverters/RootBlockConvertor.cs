using Markdig.Syntax;
using MarkdownToPdf10.MigrDoc;
using MarkdownToPdf10.Styling;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace MarkdownToPdf10.Converters.ContainerConverters;

internal class RootBlockConvertor : ContainerBlockConverter, IStandaloneContainerConverter
{
    public RootBlockConvertor(MarkdownDocument block, string rawText, MigraDocBlockContainer output,
        ContainerBlockConverter parent, MarkdownToPdf owner)
         : base(block, parent)
    {
        Owner = owner;
        RawText = rawText;
        OutputContainer = output;
        ElementDescriptor = new SingleElementDescriptor { Attributes = Attributes, Type = ElementType.Root };
        Width = Owner.RealPageWidth;
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
        Width = Owner.RealPageWidth;
        FontSize = EvaluatedStyle.Font.Size.Eval(12, Owner.RealPageWidth);
    }
}
