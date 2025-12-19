using Microsoft.Win32;
using System.Drawing.Text;

namespace VectorAi.MarkdownToPdf.MigrDoc;

public class FontFinderInfo
{
    public required string Name { get; set; }
    public required string Regular { get; set; }
    public required string Bold { get; set; }
    public required string Italic { get; set; }
    public required string BoldItalic { get; set; }
    public string Folder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts));

}

[System.Runtime.Versioning.SupportedOSPlatform("windows")]
public class WindowsFontFinder
{
    public static FontFinderInfo? Find(string familyName)
    {
        var installed = new InstalledFontCollection();
        var family = installed.Families.FirstOrDefault(f => f.Name == familyName)?.Name;
        if (family == null)
        {
            throw new InvalidOperationException($"Font family '{familyName}' is not installed.");
        }

        // Read font registry entries from HKLM and HKCU
        var entries = new System.Collections.Generic.List<(string displayName, string fileName)>();

        void ReadKey(RegistryKey key)
        {
            if (key == null) return;
            foreach (var valueName in key.GetValueNames())
            {
                try
                {
                    var val = key.GetValue(valueName) as string;
                    if (string.IsNullOrEmpty(val)) continue;
                    entries.Add((valueName, val));
                }
                catch { /* ignore individual read errors */ }
            }
        }

        using (var hkcu = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts"))
        using (var hklm = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts"))
        {
            if (hkcu != null) ReadKey(hkcu);
            if (hklm != null) ReadKey(hklm);
        }

        var matches = entries
            .Where(e => e.displayName.StartsWith(familyName, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (matches.Any())
        {
            var result = new FontFinderInfo
            {
                Name = familyName,
                Regular = matches.FirstOrDefault(m => !m.displayName.Contains("Bold") && !m.displayName.Contains("Italic")).fileName ?? "",
                Bold = matches.FirstOrDefault(m => m.displayName.Contains("Bold") && !m.displayName.Contains("Italic")).fileName ?? "",
                Italic = matches.FirstOrDefault(m => !m.displayName.Contains("Bold") && m.displayName.Contains("Italic")).fileName ?? "",
                BoldItalic = matches.FirstOrDefault(m => m.displayName.Contains("Bold") && m.displayName.Contains("Italic")).fileName ?? ""
            };
            return result;
        }

        return null;
    }
}