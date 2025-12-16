using Markdig;
using MarkdownToPdf10.Converters.ContainerConverters;
using MarkdownToPdf10.Converters.InlineConverters;
using MarkdownToPdf10.MigrDoc;
using MarkdownToPdf10.Styling;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp;
using PdfSharp.Fonts;

namespace MarkdownToPdf10;

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


    public event EventHandler<ConvertingLiteralEventArgs> ConvertingLiteral;
    public event EventHandler StylingApplied;
    public event EventHandler StylingPrepared;
    public event EventHandler<WarningEventArgs> WarningIssued;

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
        throw new NotImplementedException();
    }

    public IMarkdownToPdf AddEvenHeader(string header)
    {
        throw new NotImplementedException();
    }

    public IMarkdownToPdf AddFirstFooter(string footer)
    {
        throw new NotImplementedException();
    }

    public IMarkdownToPdf AddFirstHeader(string header)
    {
        throw new NotImplementedException();
    }

    public IMarkdownToPdf AddFooter(string footer)
    {
        throw new NotImplementedException();
    }

    public IMarkdownToPdf AddHeader(string header)
    {
        throw new NotImplementedException();
    }

    public IMarkdownToPdf AddOddFooter(string footer)
    {
        throw new NotImplementedException();
    }

    public IMarkdownToPdf AddOddHeader(string header)
    {
        throw new NotImplementedException();
    }

    public IMarkdownToPdf AddSection(bool useDefaultPageSetup = false)
    {
        throw new NotImplementedException();
    }

    public IMarkdownToPdf Author(string text)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public IMarkdownToPdf DefalutDpi(double dpi)
    {
        throw new NotImplementedException();
    }

    public IMarkdownToPdf DefaultFont(string fontName, double fontSize)
    {
        throw new NotImplementedException();
    }

    public IMarkdownToPdf FirstPageNumber(int pageNumber)
    {
        throw new NotImplementedException();
    }

    public IMarkdownToPdf FontDir(string fontDir)
    {
        throw new NotImplementedException();
    }

    public IMarkdownToPdf FooterDistance(Dimension distance)
    {
        throw new NotImplementedException();
    }

    public IMarkdownToPdf HeaderDistance(Dimension distance)
    {
        throw new NotImplementedException();
    }

    public IMarkdownToPdf HeadingScale(double scale = 1.125)
    {
        throw new NotImplementedException();
    }

    public IMarkdownToPdf ImageDir(string imageDir)
    {
        throw new NotImplementedException();
    }

    public IMarkdownToPdf PageMargins(Dimension left, Dimension right, Dimension top, Dimension bottom)
    {
        throw new NotImplementedException();
    }

    public IMarkdownToPdf PaperOrientation(PaperOrientation po)
    {
        throw new NotImplementedException();
    }

    public IMarkdownToPdf PaperSize(PaperSize ps)
    {
        throw new NotImplementedException();
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
    public IMarkdownToPdf RegisterLocalFont(string name, string regular, string bold = "", string italic = "", string boldItalic = "")
    {
        (GlobalFontSettings.FontResolver as FontResolver).Register(name, regular, bold, italic, boldItalic);
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
        throw new NotImplementedException();
    }


    public MarkdownToPdf()
    {
        ConversionSettings = new ConversionSettings();
        MigraDocument = new Document();
        DefaultPageSetup = MigraDocument.DefaultPageSetup.Clone();
        StyleManager = new StyleManager(this);
        PluginManager = new PluginManager(this);
        GlobalFontSettings.FontResolver = GlobalFontSettings.FontResolver ?? new FontResolver("C:\\Windows\\Fonts");
        GlobalFontSettings.FontResolver = (GlobalFontSettings.FontResolver as FontResolver) ?? new FontResolver("C:\\Windows\\Fonts");

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

    private Section? GetOrCreateSection()
    {
        if (MigraDocument.LastSection == null)
        {
            var section = MigraDocument.AddSection();
            section.PageSetup = DefaultPageSetup.Clone();
        }
        return MigraDocument.LastSection;
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
        PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true)
        {
            Document = MigraDocument
        };

        pdfRenderer.RenderDocument();
        return pdfRenderer;
    }

    #endregion Private methods
}
