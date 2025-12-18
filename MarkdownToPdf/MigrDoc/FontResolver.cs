// This file is a part of MarkdownToPdf Library by Geert-Jan Thomas based on earlier work by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using PdfSharp.Fonts;

namespace VectorAi.MarkdownToPdf.MigrDoc;

internal class FontResolver : IFontResolver
{
    private readonly List<FontFamily> _fonts;
    public string Dir { get; set; } = "";

    public FontResolver()
    {
        _fonts = new List<FontFamily>();
    }

    public FontResolver(string dir) : this()
    {
        Dir = dir ?? "";
    }

    public FontResolverInfo? ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        Dir = Dir ?? "";
        var name = familyName.ToLower();

        var registeredFont = _fonts.FirstOrDefault(x => x.Name.ToLower() == name);

        if (registeredFont == null)
        {
            var fnt = PlatformFontResolver.ResolveTypeface(familyName, isBold, isItalic);
            return fnt;
        }

        if (isBold)
        {
            if (isItalic)
            {
                if (registeredFont.Normal == registeredFont.BoldItalic) return new FontResolverInfo(registeredFont.Normal, PdfSharp.Drawing.XStyleSimulations.BoldItalicSimulation);
                if (registeredFont.Italic == registeredFont.BoldItalic) return new FontResolverInfo(registeredFont.Italic, PdfSharp.Drawing.XStyleSimulations.BoldSimulation);
                if (registeredFont.Bold == registeredFont.BoldItalic) return new FontResolverInfo(registeredFont.Bold, PdfSharp.Drawing.XStyleSimulations.ItalicSimulation);
                return new FontResolverInfo(registeredFont.BoldItalic);
            }

            if (registeredFont.Normal == registeredFont.Bold) return new FontResolverInfo(registeredFont.Normal, PdfSharp.Drawing.XStyleSimulations.BoldSimulation);
            return new FontResolverInfo(registeredFont.Bold);
        }
        if (isItalic)
        {
            if (registeredFont.Normal == registeredFont.Italic) return new FontResolverInfo(registeredFont.Normal, PdfSharp.Drawing.XStyleSimulations.ItalicSimulation);
            return new FontResolverInfo(registeredFont.Italic);
        }

        return new FontResolverInfo(registeredFont.Normal);
    }

    public void Register(string name, string regular, string bold = "", string italic = "", string boldItalic = "", string? folder = null)
    {
        var existing = _fonts.FirstOrDefault(x => x.Name == name);

        if (existing != null)
        {
            _fonts.RemoveAt(_fonts.IndexOf(existing));
        }

        _fonts.Add(new FontFamily(name, regular, bold, italic, boldItalic, folder));
    }

    public byte[] GetFont(string faceName)
    {
        if (File.Exists(faceName))
        {
            return File.ReadAllBytes(faceName);
        }

        //if (!(Dir.EndsWith(Path.DirectorySeparatorChar.ToString()) || Dir.EndsWith(Path.AltDirectorySeparatorChar.ToString())))
        //{
        //    Dir += Path.DirectorySeparatorChar;
        //}
        var fullPath = Path.Combine(Dir, faceName);
        if (!File.Exists(fullPath))
        {
            fullPath = Path.Combine(_fonts.FirstOrDefault(f => 
                f.Normal == faceName
                || f.Bold == faceName
                || f.Italic == faceName
                || f.BoldItalic == faceName
                )?.Folder ?? "", faceName);
        }
        return File.ReadAllBytes(fullPath);


    }
}
