using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace MarkdownToPdf.Tests.Utils;
public class MarkdigExtensions_Tests : IDisposable
{
    MarkdownDocument blockDocument = new();
    ParagraphBlock block1 = new();
    ParagraphBlock block2 = new();
    ParagraphBlock block3 = new();
    ParagraphBlock blockX = new();

    ContainerInline container = new();
    LiteralInline inline1 = new();
    LiteralInline inline2 = new();
    LiteralInline inline3 = new();
    LiteralInline inlineX = new();

    public MarkdigExtensions_Tests()
    {
        // Arrange
        blockDocument.Add(block1);
        blockDocument.Add(block2);
        blockDocument.Add(block3);

        container.AppendChild(inline1);
        container.AppendChild(inline2);
        container.AppendChild(inline3);
    }

    public void Dispose()
    {
    }

    [Fact]
    public void IsFirst_Block()
    {
        // Act
        var block1first = VectorAi.MarkdownToPdf.Utils.MarkdigExtensions.IsFirst(block1);
        var block2first = VectorAi.MarkdownToPdf.Utils.MarkdigExtensions.IsFirst(block2);
        var block3first = VectorAi.MarkdownToPdf.Utils.MarkdigExtensions.IsFirst(block3);
        var blockXfirst = VectorAi.MarkdownToPdf.Utils.MarkdigExtensions.IsFirst(blockX);


        // Assert
        Assert.True(block1first);
        Assert.False(block2first);
        Assert.False(block3first);
        Assert.False(blockXfirst);
    }

    [Fact]
    public void IsLast_Block()
    {
        // Act
        var block1last = VectorAi.MarkdownToPdf.Utils.MarkdigExtensions.IsLast(block1);
        var block2last = VectorAi.MarkdownToPdf.Utils.MarkdigExtensions.IsLast(block2);
        var block3last = VectorAi.MarkdownToPdf.Utils.MarkdigExtensions.IsLast(block3);
        var blockXlast = VectorAi.MarkdownToPdf.Utils.MarkdigExtensions.IsLast(blockX);


        // Assert
        Assert.False(block1last);
        Assert.False(block2last);
        Assert.True(block3last);
        Assert.False(blockXlast);
    }

    [Fact]
    public void GetIndex_Block()
    {
        // Act
        var block1idx = VectorAi.MarkdownToPdf.Utils.MarkdigExtensions.GetIndex(block1);
        var block2idx = VectorAi.MarkdownToPdf.Utils.MarkdigExtensions.GetIndex(block2);
        var block3idx = VectorAi.MarkdownToPdf.Utils.MarkdigExtensions.GetIndex(block3);
        var blockXidx = VectorAi.MarkdownToPdf.Utils.MarkdigExtensions.GetIndex(blockX);


        // Assert
        Assert.Equal(0, block1idx);
        Assert.Equal(1, block2idx);
        Assert.Equal(2, block3idx);
        Assert.Equal(-1, blockXidx);
    }

    [Fact]
    public void IsFirst_Inline()
    {
        // Act
        var inline1first = VectorAi.MarkdownToPdf.Utils.MarkdigExtensions.IsFirst(inline1);
        var inline2first = VectorAi.MarkdownToPdf.Utils.MarkdigExtensions.IsFirst(inline2);
        var inline3first = VectorAi.MarkdownToPdf.Utils.MarkdigExtensions.IsFirst(inline3);
        var inlineXfirst = VectorAi.MarkdownToPdf.Utils.MarkdigExtensions.IsFirst(inlineX);


        // Assert
        Assert.True (inline1first);
        Assert.False(inline2first);
        Assert.False(inline3first);
        Assert.False(inlineXfirst);
    }

    [Fact]
    public void IsLast_Inline()
    {
        // Act
        var inline1last = VectorAi.MarkdownToPdf.Utils.MarkdigExtensions.IsLast(inline1);
        var inline2last = VectorAi.MarkdownToPdf.Utils.MarkdigExtensions.IsLast(inline2);
        var inline3last = VectorAi.MarkdownToPdf.Utils.MarkdigExtensions.IsLast(inline3);
        var inlineXlast = VectorAi.MarkdownToPdf.Utils.MarkdigExtensions.IsLast(inlineX);


        // Assert
        Assert.False(inline1last);
        Assert.False(inline2last);
        Assert.True (inline3last);
        Assert.False(inlineXlast);
    }

    [Fact]
    public void GetIndex_Inline()
    {
        // Act
        var inline1idx = VectorAi.MarkdownToPdf.Utils.MarkdigExtensions.GetIndex(inline1);
        var inline2idx = VectorAi.MarkdownToPdf.Utils.MarkdigExtensions.GetIndex(inline2);
        var inline3idx = VectorAi.MarkdownToPdf.Utils.MarkdigExtensions.GetIndex(inline3);
        var inlineXidx = VectorAi.MarkdownToPdf.Utils.MarkdigExtensions.GetIndex(inlineX);


        // Assert
        Assert.Equal(0, inline1idx);
        Assert.Equal(1, inline2idx);
        Assert.Equal(2, inline3idx);
        Assert.Equal(-1, inlineXidx);
    }
}
