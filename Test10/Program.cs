using PdfSharp.Fonts;
using System;
using System.Drawing.Text;
using System.Linq;
using System.IO;
using Microsoft.Win32;
using System.Text;

namespace Test10;

internal class Program
{
    private static void Main(string[] args)
    {
        // get all windows fonts
        ListAllWindowsFonts();

        RunAllExamples();
    }

    private static void ListAllWindowsFonts()
    {
        try
        {
            var installed = new InstalledFontCollection();
            var families = installed.Families.OrderBy(f => f.Name).ToList();

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
                ReadKey(hkcu);
                ReadKey(hklm);
            }

            var fontsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Fonts");

            Console.WriteLine("Installed Windows fonts (family -> files with style classification):\n");

            foreach (var family in families)
            {
                Console.WriteLine(family.Name + ":");

                // first try exact prefix matches, then fallback to contains
                var matches = entries
                    .Where(e => e.displayName.StartsWith(family.Name, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (!matches.Any())
                {
                    matches = entries
                        .Where(e => e.displayName.IndexOf(family.Name, StringComparison.OrdinalIgnoreCase) >= 0)
                        .ToList();
                }

                var files = new System.Collections.Generic.List<(string path, string displayName)>();

                foreach (var m in matches)
                {
                    // value may contain multiple filenames separated by comma
                    var parts = m.fileName.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var p in parts)
                    {
                        var file = p.Trim();
                        if (string.IsNullOrEmpty(file)) continue;

                        // if registry contains a path already use it, otherwise assume Fonts folder
                        string fullPath = file;
                        try
                        {
                            if (!Path.IsPathRooted(fullPath))
                                fullPath = Path.Combine(fontsDir, fullPath);
                        }
                        catch { }

                        if (!files.Any(x => string.Equals(x.path, fullPath, StringComparison.OrdinalIgnoreCase)))
                            files.Add((fullPath, m.displayName));
                    }
                }

                if (files.Count == 0)
                {
                    Console.WriteLine("  (no registry mapping found)");
                }
                else
                {
                    foreach (var entry in files)
                    {
                        var f = entry.path;
                        var display = entry.displayName;
                        if (!File.Exists(f))
                        {
                            Console.WriteLine("  " + f + " (missing)");
                            continue;
                        }

                        string subfamily = null;
                        try
                        {
                            subfamily = GetFontSubfamily(f);
                        }
                        catch (Exception ex)
                        {
                            // ignore parse errors and fallback to heuristic
                            subfamily = null;
                        }

                        var style = ClassifyStyle(subfamily, display, f);
                        Console.WriteLine($"  {f}  -> {style}  (subfamily: {subfamily ?? "?"})");
                    }
                }

                Console.WriteLine();
            }

            Console.WriteLine($"Total font families: {families.Count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to enumerate installed fonts: " + ex.Message);
        }
    }

    // Try to read the 'name' table subfamily (nameID = 2) from a TTF/OTF file.
    private static string GetFontSubfamily(string path)
    {
        using var fs = File.OpenRead(path);
        using var br = new BinaryReader(fs);

        // Read header tag
        var tagBytes = br.ReadBytes(4);
        if (tagBytes.Length < 4) return null;
        var tag = Encoding.ASCII.GetString(tagBytes);
        long sfntOffset = 0;

        if (tag == "ttcf")
        {
            // TrueType collection - read first font offset and use it
            // ttcf header: 4 bytes 'ttcf', 4 bytes version, 4 bytes numFonts, then array of offsets
            _ = ReadUInt32BE(br); // version
            uint numFonts = ReadUInt32BE(br);
            if (numFonts == 0) return null;
            uint firstOffset = ReadUInt32BE(br);
            sfntOffset = firstOffset;
        }
        else
        {
            // Not a collection, sfnt at offset 0
            sfntOffset = 0;
        }

        fs.Seek(sfntOffset, SeekOrigin.Begin);

        // Read sfnt header
        var sfntVersion = ReadUInt32BE(br);
        ushort numTables = ReadUInt16BE(br);
        // skip searchRange, entrySelector, rangeShift
        _ = ReadUInt16BE(br);
        _ = ReadUInt16BE(br);
        _ = ReadUInt16BE(br);

        uint nameTableOffset = 0;
        uint nameTableLength = 0;

        for (int i = 0; i < numTables; i++)
        {
            var tableTagBytes = br.ReadBytes(4);
            var tableTag = Encoding.ASCII.GetString(tableTagBytes);
            uint checksum = ReadUInt32BE(br);
            uint offset = ReadUInt32BE(br);
            uint length = ReadUInt32BE(br);

            if (tableTag == "name")
            {
                nameTableOffset = offset;
                nameTableLength = length;
            }
        }

        if (nameTableOffset == 0) return null;

        fs.Seek(nameTableOffset, SeekOrigin.Begin);
        ushort formatSelector = ReadUInt16BE(br);
        ushort count = ReadUInt16BE(br);
        ushort stringOffset = ReadUInt16BE(br); // from start of name table

        var records = new System.Collections.Generic.List<(ushort platformID, ushort encodingID, ushort languageID, ushort nameID, ushort length, ushort offset)>();

        for (int i = 0; i < count; i++)
        {
            ushort platformID = ReadUInt16BE(br);
            ushort encodingID = ReadUInt16BE(br);
            ushort languageID = ReadUInt16BE(br);
            ushort nameID = ReadUInt16BE(br);
            ushort lengthRec = ReadUInt16BE(br);
            ushort offsetRec = ReadUInt16BE(br);
            records.Add((platformID, encodingID, languageID, nameID, lengthRec, offsetRec));
        }

        // string storage start
        long storageStart = nameTableOffset + stringOffset;

        // prefer Windows platform (3) and English language 0x0409
        var candidates = records.Where(r => r.nameID == 2).ToList();
        if (!candidates.Any()) return null;

        (ushort platformID, ushort encodingID, ushort languageID, ushort nameID, ushort length, ushort offset) selected = default;
        bool found = false;

        // try Windows English
        selected = candidates.FirstOrDefault(r => r.platformID == 3 && (r.languageID == 0x0409 || r.languageID == 0));
        if (selected.length != 0) found = true;

        if (!found)
        {
            // any Windows
            selected = candidates.FirstOrDefault(r => r.platformID == 3);
            if (selected.length != 0) found = true;
        }

        if (!found)
        {
            // any platform
            selected = candidates.FirstOrDefault();
            if (selected.length != 0) found = true;
        }

        if (!found) return null;

        fs.Seek(storageStart + selected.offset, SeekOrigin.Begin);
        var data = br.ReadBytes(selected.length);

        string value = null;
        try
        {
            if (selected.platformID == 3 || selected.platformID == 0)
            {
                // UTF-16BE
                value = Encoding.BigEndianUnicode.GetString(data);
            }
            else if (selected.platformID == 1)
            {
                // Macintosh - usually MacRoman
                try
                {
                    value = Encoding.GetEncoding("macintosh").GetString(data);
                }
                catch
                {
                    value = Encoding.ASCII.GetString(data);
                }
            }
            else
            {
                value = Encoding.UTF8.GetString(data);
            }
        }
        catch
        {
            value = null;
        }

        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static string ClassifyStyle(string subfamily, string displayName, string fileName)
    {
        var s = (subfamily ?? string.Empty).ToLowerInvariant();
        if (string.IsNullOrEmpty(s)) s = (displayName ?? string.Empty).ToLowerInvariant();

        if (s.Contains("bold") && (s.Contains("italic") || s.Contains("oblique"))) return "BoldItalic";
        if (s.Contains("bold")) return "Bold";
        if (s.Contains("italic") || s.Contains("oblique") || s.Contains("ital")) return "Italic";
        if (s.Contains("regular") || s.Contains("normal") || s.Contains("rg") || s.Contains("book")) return "Regular";

        // fallback to filename heuristics
        var n = Path.GetFileNameWithoutExtension(fileName ?? "").ToLowerInvariant();
        if (n.Contains("bold") && (n.Contains("italic") || n.Contains("oblique") || n.Contains("bi"))) return "BoldItalic";
        if (n.Contains("bold") || n.EndsWith("-b") || n.EndsWith("_b") || n.EndsWith("bd")) return "Bold";
        if (n.Contains("italic") || n.EndsWith("-i") || n.EndsWith("_i") || n.EndsWith("it")) return "Italic";

        return "Regular";
    }

    private static ushort ReadUInt16BE(BinaryReader br)
    {
        var b1 = br.ReadByte();
        var b2 = br.ReadByte();
        return (ushort)((b1 << 8) | b2);
    }

    private static uint ReadUInt32BE(BinaryReader br)
    {
        var b1 = br.ReadByte();
        var b2 = br.ReadByte();
        var b3 = br.ReadByte();
        var b4 = br.ReadByte();
        return (uint)((b1 << 24) | (b2 << 16) | (b3 << 8) | b4);
    }

    private static void RunAllExamples()
    {
        Examples.HelloWorld.Run();
        //Examples.BasicStyling.Run();
        //Examples.CustomStyles.Run();
        //Examples.AdvancedStyling.Run();
        //Examples.Tables.Run();
        //Examples.Sections.Run();
        //Examples.Events.Run();
        //Examples.Toc.Run();
        //Examples.Highlighting.Run();
        //Examples.Features.Run();
        //Examples.Attributes.Run();
        //Examples.FullBook.Run();
    }
}


