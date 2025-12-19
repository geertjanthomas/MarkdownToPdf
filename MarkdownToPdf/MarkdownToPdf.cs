// This file is a part of MarkdownToPdf Library by Geert-Jan Thomas based on earlier work by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Markdig;
using VectorAi.MarkdownToPdf.Converters.ContainerConverters;
using VectorAi.MarkdownToPdf.Converters.InlineConverters;
using VectorAi.MarkdownToPdf.MigrDoc;
using VectorAi.MarkdownToPdf.Styling;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Fonts;

namespace VectorAi.MarkdownToPdf;


public sealed class MarkdownToPdf : IMarkdownToPdf
{
    internal readonly DefaultStyles defaultStyles;
    internal readonly List<string> tempFiles = new List<string>();

    public ConversionSettings ConversionSettings { get; private set; }
    public PageSetup DefaultPageSetup { get; private set; }
    public DocumentInfo DocumentInfo { get => throw new NotImplementedException(); }

    /// <summary>
    /// MigraDoc output document. Can be tweaked directly if needed.
    /// </summary>
    public Document MigraDocument { get; private set; }

    public PluginManager PluginManager { get; private set; }
    public StyleManager StyleManager { get; private set; }

    /// <summary>
    /// Calculated PageWidth without margins
    /// </summary>
    internal Unit RealPageWidth { get => GetRealPageWidth(); }


    public event EventHandler<ConvertingLiteralEventArgs>? ConvertingLiteral;
    public event EventHandler? StylingApplied;
    public event EventHandler? StylingPrepared;
    public event EventHandler<WarningEventArgs>? WarningIssued;

    internal IMarkdownToPdf Add(MigraDocBlockContainer container, string markdownText)
    {
        var md = Markdown.Parse(markdownText, ConversionSettings.Pipeline);
        var conv = new RootBlockConvertor(md, markdownText, container, null, this);
        conv.Convert();
        return this;
    }

    /// <summary>
    /// Adds markdown text to body (main document part) of current section. If it does not exist, it is created first;
    /// </summary>
    public IMarkdownToPdf Add(string markdownText)
    {
        var section = GetOrCreateSection();
        var container = new MigraDocBlockContainer(section, this);
        return Add(container, markdownText);
    }

    public IMarkdownToPdf AddEvenFooter(string footer)
    {
        var section = GetOrCreateSection();
        section.PageSetup.OddAndEvenPagesHeaderFooter = true;
        var container = new MigraDocBlockContainer(section.Footers.EvenPage, this);
        return Add(container, footer);
    }

    public IMarkdownToPdf AddEvenHeader(string header)
    {
        var section = GetOrCreateSection();
        section.PageSetup.OddAndEvenPagesHeaderFooter = true;
        var container = new MigraDocBlockContainer(section.Headers.EvenPage, this);
        return Add(container, header);
    }

    public IMarkdownToPdf AddFirstFooter(string footer)
    {
        var section = GetOrCreateSection();
        var container = new MigraDocBlockContainer(section.Footers.FirstPage, this);
        section.PageSetup.DifferentFirstPageHeaderFooter = true;
        return Add(container, footer);
    }

    public IMarkdownToPdf AddFirstHeader(string header)
    {
        var section = GetOrCreateSection();
        var container = new MigraDocBlockContainer(section.Headers.FirstPage, this);
        section.PageSetup.DifferentFirstPageHeaderFooter = true;
        return Add(container, header);
    }

    public IMarkdownToPdf AddFooter(string footer)
    {
        var section = GetOrCreateSection();
        var container = new MigraDocBlockContainer(section.Footers.Primary, this);
        return Add(container, footer);
    }

    public IMarkdownToPdf AddHeader(string header)
    {
        var section = GetOrCreateSection();
        var container = new MigraDocBlockContainer(section.Headers.Primary, this);
        return Add(container, header);
    }

    public IMarkdownToPdf AddOddFooter(string footer)
    {
        var section = GetOrCreateSection();
        section.PageSetup.OddAndEvenPagesHeaderFooter = true;
        return AddFooter(footer);
    }

    public IMarkdownToPdf AddOddHeader(string header)
    {
        var section = GetOrCreateSection();
        section.PageSetup.OddAndEvenPagesHeaderFooter = true;
        return AddHeader(header);
    }

    public IMarkdownToPdf AddSection(bool useDefaultPageSetup = false)
    {
        var section = MigraDocument.AddSection();

        if (useDefaultPageSetup || MigraDocument.Sections.Count == 1)
        {
            section.PageSetup = DefaultPageSetup.Clone();
        }
        return this;
    }

    public IMarkdownToPdf Author(string text)
    {
        MigraDocument.Info.Author = text;
        return this;
    }

    public void Clear()
    {
        MigraDocument = new Document();

        foreach (var f in tempFiles.Where(x => File.Exists(x)))
        {
            File.Delete(f);
        }
    }

    public IMarkdownToPdf DefaultDpi(double dpi)
    {
        ConversionSettings.DefaultDpi = dpi;
        return this;
    }

    public IMarkdownToPdf DefaultFont(string fontName, double fontSize)
    {
        defaultStyles.SetDefaultFont(fontName, fontSize);
        return this;
    }

    public IMarkdownToPdf FirstPageNumber(int pageNumber)
    {
        PageSetup target = GetCurrentPageSetup();
        target.StartingNumber = pageNumber;
        return this;
    }

    public IMarkdownToPdf FontDir(string fontDir)
    {
        ConversionSettings.FontDir = fontDir;
        return this;
    }

    public IMarkdownToPdf FooterDistance(Dimension distance)
    {
        PageSetup target = GetCurrentPageSetup();
        target.FooterDistance = distance.Eval(defaultStyles._fontSize, RealPageWidth.Point);
        return this;
    }

    public IMarkdownToPdf HeaderDistance(Dimension distance)
    {
        PageSetup target = GetCurrentPageSetup();
        target.HeaderDistance = distance.Eval(defaultStyles._fontSize, RealPageWidth.Point);
        return this;
    }

    public IMarkdownToPdf HeadingScale(double scale = 1.125)
    {
        defaultStyles.InitOrUpdateHeadings(scale, fullInit: false);
        return this;
    }

    public IMarkdownToPdf ImageDir(string imageDir)
    {
        ConversionSettings.ImageDir = imageDir;
        return this;
    }

    public IMarkdownToPdf PageMargins(Dimension left, Dimension right, Dimension top, Dimension bottom)
    {
        PageSetup target = GetCurrentPageSetup();
        target.LeftMargin = left.Eval(defaultStyles._fontSize, RealPageWidth.Point);
        target.RightMargin = right.Eval(defaultStyles._fontSize, RealPageWidth.Point);
        target.TopMargin = top.Eval(defaultStyles._fontSize, RealPageWidth.Point);
        target.BottomMargin = bottom.Eval(defaultStyles._fontSize, RealPageWidth.Point);
        return this;
    }

    public IMarkdownToPdf PaperOrientation(PaperOrientation po)
    {
        PageSetup target = GetCurrentPageSetup();
        target.Orientation = (Orientation)po;
        return this;
    }

    public IMarkdownToPdf PaperSize(PaperSize ps)
    {
        PageSetup target = GetCurrentPageSetup();
        var sizes = new Dictionary<PaperSize, (double Width, double Height)>()
            {
                { VectorAi.MarkdownToPdf.PaperSize.A0, (841, 1189)},
                { VectorAi.MarkdownToPdf.PaperSize.A1, (594, 841)},
                { VectorAi.MarkdownToPdf.PaperSize.A2, (420, 594)},
                { VectorAi.MarkdownToPdf.PaperSize.A3, (297, 420)},
                { VectorAi.MarkdownToPdf.PaperSize.A4, (210, 297)},
                { VectorAi.MarkdownToPdf.PaperSize.A5, (148, 210)},
                { VectorAi.MarkdownToPdf.PaperSize.A6, (105, 148)},
                { VectorAi.MarkdownToPdf.PaperSize.B0, (1000, 1414)},
                { VectorAi.MarkdownToPdf.PaperSize.B1, (707, 1000)},
                { VectorAi.MarkdownToPdf.PaperSize.B2, (500, 707)},
                { VectorAi.MarkdownToPdf.PaperSize.B3, (353, 500)},
                { VectorAi.MarkdownToPdf.PaperSize.B4, (250, 353)},
                { VectorAi.MarkdownToPdf.PaperSize.B5, (176, 250)},
                { VectorAi.MarkdownToPdf.PaperSize.B6, (125, 176)},
                { VectorAi.MarkdownToPdf.PaperSize.Letter, (216, 279)},
                { VectorAi.MarkdownToPdf.PaperSize.Legal, (216, 356)},
                { VectorAi.MarkdownToPdf.PaperSize.Ledger, (279, 432)},
                { VectorAi.MarkdownToPdf.PaperSize.Tabloid, (279, 432)},
                { VectorAi.MarkdownToPdf.PaperSize.P11x17, (11 * 25.4, 17 * 25.4)}
            };
        target.PageWidth = Unit.FromMillimeter(sizes[ps].Width);
        target.PageHeight = Unit.FromMillimeter(sizes[ps].Height);

        if (ps < VectorAi.MarkdownToPdf.PaperSize.B0)
            target.PageFormat = (PageFormat)ps;
        return this;
    }

    /// <summary>
    /// This method enables use of local (non-system) fonts in the output
    /// </summary>
    /// <param name="name">Name to be used in styles</param>
    /// <param name="regular">Filename of regular typeface font</param>
    /// <param name="bold">Filename of bold typeface font. If not provided, the typeface is automatically generated from regular</param>
    /// <param name="italic">Filename of italic typeface font. If not provided, the typeface is automatically generated from regular</param>
    /// <param name="boldItalic">Filename of bold and italic typeface font. If not provided, the typeface is automatically generated from other provided fonts</param>
    /// <returns></returns>
    public IMarkdownToPdf RegisterLocalFont(string name, string regular, string bold = "", string italic = "", string boldItalic = "", string? folder = null)
    {
        (GlobalFontSettings.FontResolver as FontResolver)?.Register(name, regular, bold, italic, boldItalic, folder);
        return this;
    }

    public void Save(Stream stream)
    {
        PdfDocumentRenderer pdfRenderer = Render();
        pdfRenderer.PdfDocument.Save(stream);
    }

    public void Save(string filename)
    {
        PdfDocumentRenderer pdfRenderer = Render();
        pdfRenderer.PdfDocument.Save(filename);
    }

    public IMarkdownToPdf Title(string text)
    {
        MigraDocument.Info.Title = text;
        return this;
    }


    public MarkdownToPdf()
    {
        ConversionSettings = new ConversionSettings();
        MigraDocument = new Document();
        DefaultPageSetup = MigraDocument.DefaultPageSetup.Clone();
        StyleManager = new StyleManager(this);
        PluginManager = new PluginManager(this);
        GlobalFontSettings.FontResolver = GlobalFontSettings.FontResolver ?? new FontResolver();
        GlobalFontSettings.FontResolver = (GlobalFontSettings.FontResolver as FontResolver) ?? new FontResolver();

        defaultStyles = new DefaultStyles(StyleManager);
        defaultStyles.CreateBasicStyles();
        defaultStyles.BindBasicStyles();
        defaultStyles.InitBasicStyles();
        defaultStyles.InitSmartyPants();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Dispose(bool disposing)
    {
        Clear();
    }




    #region Event Invokers

    internal void OnStylingPrepared(object o) => StylingPrepared?.Invoke(o, EventArgs.Empty);

    internal void OnStylingApplied(object o) => StylingApplied?.Invoke(o, EventArgs.Empty);

    internal string OnConvertingLiteral(object o, string text)
    {
        var args = new ConvertingLiteralEventArgs { Text = text };
        ConvertingLiteral?.Invoke(o, args);
        return args.Text;
    }

    internal void OnWarningIssued(object o, string category, string message) =>
        WarningIssued?.Invoke(o, new WarningEventArgs { Category = category, Message = message });

    #endregion Event Invokers

    #region Private methods

    private PageSetup GetCurrentPageSetup()
    {
        PageSetup target;

        if (MigraDocument.LastSection == null)
        {
            target = DefaultPageSetup;
        }
        else
        {
            target = MigraDocument.LastSection.PageSetup;
        }

        return target;
    }

    private Section GetOrCreateSection()
    {
        if (MigraDocument.LastSection == null)
        {
            var section = MigraDocument.AddSection();
            section.PageSetup = DefaultPageSetup.Clone();
        }
        return MigraDocument.LastSection ?? throw new Exception("No section in document");
    }

    private Unit GetRealPageWidth()
    {
        PageSetup target = GetCurrentPageSetup();
        var width = target.PageWidth;
        width = width.IsEmpty ? DefaultPageSetup.PageWidth : width;

        var lm = target.LeftMargin;
        lm = lm.IsEmpty ? DefaultPageSetup.LeftMargin : lm;
        var rm = target.RightMargin;
        rm = rm.IsEmpty ? DefaultPageSetup.RightMargin : rm;

        var res = width - lm - rm;

        if (res <= 0) throw new InvalidOperationException("Invalid page size");
        return res;
    }

    private PdfDocumentRenderer Render()
    {
        PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer()
        {
            Document = MigraDocument
        };
        pdfRenderer.RenderDocument();
        return pdfRenderer;
    }

    #endregion Private methods
}
