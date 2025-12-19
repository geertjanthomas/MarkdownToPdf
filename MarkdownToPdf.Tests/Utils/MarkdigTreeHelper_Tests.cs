using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using VectorAi.MarkdownToPdf.Utils;

namespace MarkdownToPdf.Tests.Utils;

public class MarkdigTreeHelper_Tests
{
    private LinkInline GetLinkInline(string markdown)
    {
        var pipeline = new MarkdownPipelineBuilder().Build();
        var document = Markdown.Parse(markdown, pipeline);

        // Descendants provides a flattened list of all elements in the AST
        return document.Descendants<LinkInline>().FirstOrDefault() ?? throw new NullReferenceException();
    }

    [Fact]
    public void IsOnlyBlockElement_SingleLinkInParagraph_ReturnsTrue()
    {
        // Arrange
        var markdown = "[link](https://example.com)";
        var link = GetLinkInline(markdown);

        // Act
        var result = MarkdigTreeHelper.IsOnlyBlockElement(link);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsOnlyBlockElement_LinkWithTrailingText_ReturnsFalse()
    {
        // Arrange
        var markdown = "[link](https://example.com) some text";
        var link = GetLinkInline(markdown);

        // Act
        var result = MarkdigTreeHelper.IsOnlyBlockElement(link);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsOnlyBlockElement_LinkWithLeadingText_ReturnsFalse()
    {
        // Arrange
        var markdown = "some text [link](https://example.com)";
        var link = GetLinkInline(markdown);

        // Act
        var result = MarkdigTreeHelper.IsOnlyBlockElement(link);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsOnlyTopParagraphElement_SingleLinkInTopLevelParagraph_ReturnsTrue()
    {
        // Arrange
        var markdown = "[link](https://example.com)";
        var link = GetLinkInline(markdown);

        // Act
        var result = MarkdigTreeHelper.IsOnlyTopParagraphElement(link);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsOnlyTopParagraphElement_LinkInNestedList_ReturnsFalse()
    {
        // Arrange
        // This puts the Link -> ContainerInline -> Paragraph -> ListItem -> ListBlock -> Document
        var markdown = "- [link](https://example.com)";
        var link = GetLinkInline(markdown);

        // Act
        var result = MarkdigTreeHelper.IsOnlyTopParagraphElement(link);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsOnlyTopParagraphElement_LinkInQuoteBlock_ReturnsFalse()
    {
        // Arrange
        var markdown = "> [link](https://example.com)";
        var link = GetLinkInline(markdown);

        // Act
        var result = MarkdigTreeHelper.IsOnlyTopParagraphElement(link);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsOnlyTopParagraphElement_MultipleParagraphs_FirstIsOnlyInItsBlock_ReturnsTrue()
    {
        // Arrange
        var markdown = @"[link1](https://example.com)

Second paragraph";
        var link = GetLinkInline(markdown);

        // Act
        var result = MarkdigTreeHelper.IsOnlyTopParagraphElement(link);

        // Assert
        Assert.True(result);
    }
}
