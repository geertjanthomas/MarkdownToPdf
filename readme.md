# MarkdownToPdf Library

MarkdownToPdf is a .NET library serving as a converter of markdown text to PDF. It supports fully customizable styling via cascading styles, page setup, headers and footers, page numbering, sections, page and sections breaks  and it also supports plugins for features like syntax highlighting or for displaying mathematical expressions. Technically it uses [markdig](https://github.com/xoofx/markdig) library to parse the markdown document and [Pdfsharp/Migradoc](http://www.pdfsharp.net/) library to render the output.
It also uses [ImageSharp from SixLabors](https://sixlabors.com/products/imagesharp/) to handle cross platform image rendering.

## Disclaimer
This is a fresh fork from [https://github.com/tkubec/MarkdownToPdf](https://github.com/tkubec/MarkdownToPdf) and upgraded to .NET 10.

Not yet everything is ported and tested fully. 

The aim is to keep up the support for new .NET versions and to ensure cross platform compatibility

# Supported markdown flavors and extensions

 - basic and github  markdown
 - Pipe tables
 - Grid tables 
 - Extra emphasis    (strike through ~~,Subscript ~ Superscript ^ Inserted ++ Marked ==)
 - Special attributes for applying styles and formatting
 - Footnotes 
 - Task Lists 
 - Citation text by enclosing ""..."" 
 - Custom containers similar to fenced code block :::
 - Mathematics/Latex extension by enclosing $$ for block and $ for inline math
 - SmartyPants 

For details, see [markdig documentation](https://github.com/xoofx/markdig)  and this [project documentation](../../wiki/MarkdownToPdf-Library).

# Basic Usage

The basic usage is pretty straightforward:

- create an instance of `MarkdownToPdf`
- optionally set-up the [page layout](../../wiki/Page-Setup)
- optionally modify or add [styles](../../wiki/styles)
- add the markdown text
- save

E.g.:

```csharp

var pdf = new MarkdownToPdf();

pdf
    .Add("# Hello, Wolrd")
    .Save("output.pdf");

```

Please see the documentation for more information. Here are some sample PDF outputs: 
 
 - [Feature Overview (PDF)](https://github.com/geertjanthomas/MarkdownToPdf/blob/main/Tests/output/features.pdf)
 
 - [Entire book (PDF)](https://github.com/geertjanthomas/MarkdownToPdf/blob/master/Tests/output/book_alice.pdf)
 
# Installation

The library is available as a NuGet package: [![](https://img.shields.io/badge/nuget-v1.0-blue)](https://www.nuget.org/packages/VectorAi.MarkdownToPdf)

# Plugins

## Syntax highlighter
[MarkdownToPdf.SyntaxHighlighter](https://github.com/geertjanthomas/MarkdownToPdf.SyntaxHighlighter) based on PrismSharp currently supports over 270 programming languages and has 44 built in visual themes

# License
This software is released under the MIT license.
