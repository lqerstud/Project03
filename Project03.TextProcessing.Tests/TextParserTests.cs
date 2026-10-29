using Xunit;

public class TextParserTests
{
    [Fact]
    public void Parse_NullOrWhitespace_ReturnsEmptyText()
    {
        Assert.Empty(TextParser.Parse(null!).Sentences);
        Assert.Empty(TextParser.Parse("").Sentences);
        Assert.Empty(TextParser.Parse("   \t\r\n  ").Sentences);
    }

    [Fact]
    public void Parse_SingleSentence_ProducesOneSentence()
    {
        var text = TextParser.Parse("Hello world.");
        var s = Assert.Single(text.Sentences);
        Assert.Equal(2, s.WordCount);
        Assert.Equal("Hello world.", s.ToString());
    }

    [Fact]
    public void Parse_MultipleSentences_SplitByTerminators()
    {
        var text = TextParser.Parse("One. Two! Three?");
        Assert.Equal(3, text.Sentences.Count);
        Assert.Equal("One.", text.Sentences[0].ToString());
        Assert.Equal("Two!",    text.Sentences[1].ToString());
        Assert.Equal("Three?",  text.Sentences[2].ToString());
    }

    [Fact]
    public void Parse_RussianText_Works()
    {
        var text = TextParser.Parse("Мама мыла раму. Кто взял книгу?");
        Assert.Equal(2, text.Sentences.Count);
        Assert.Equal(3, text.Sentences[0].WordCount);
        Assert.True(text.Sentences[1].IsInterrogative);
    }

    [Fact]
    public void Parse_PunctuationAttachedToWord_IsSeparateToken()
    {
        var text = TextParser.Parse("Hello, world!");
        var s = text.Sentences[0];
        Assert.Equal(2, s.WordCount);       // Hello, world
        Assert.Equal(2, s.Tokens.Count(t => t is Punctuation)); // , и !
    }

    [Fact]
    public void Parse_WordWithApostropheAndHyphen_IsSingleToken()
    {
        var text = TextParser.Parse("Don't stop well-known.");
        var s = text.Sentences[0];
        Assert.Contains(s.Words, w => w.Value == "Don't");
        Assert.Contains(s.Words, w => w.Value == "well-known");
    }

    [Fact]
    public void Parse_NewlinesAlsoSplitSentences()
    {
        var text = TextParser.Parse("Line one\nLine two");
        Assert.Equal(2, text.Sentences.Count);
    }

    [Fact]
    public void Parse_WordLengthIsCharacterLength()
    {
        var text = TextParser.Parse("Hello.");
        Assert.Equal(5, text.Sentences[0].Words.First().Length);
    }
}