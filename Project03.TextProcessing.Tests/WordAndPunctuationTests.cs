using Xunit;

public class WordAndPunctuationTests
{
    [Theory]
    [InlineData("Mary",  true)]   // M — согласная
    [InlineData("Apple", false)]  // A — гласная
    [InlineData("ёж",    false)]  // ё — гласная
    [InlineData("Мама",  true)]   // М — согласная
    [InlineData("окно",  false)]  // о — гласная
    [InlineData("Яблоко", false)] // Я — гласная
    [InlineData("",      false)]  // пустое слово
    [InlineData("123",   false)]  // не буква
    public void Word_StartsWithConsonant_DetectsCorrectly(string content, bool expected)
    {
        var w = new Word(content);
        Assert.Equal(expected, w.StartsWithConsonant);
    }

    [Fact]
    public void Word_Length_And_Value()
    {
        var w = new Word("hello");
        Assert.Equal(5, w.Length);
        Assert.Equal("hello", w.Value);
    }

    [Fact]
    public void Word_DefaultConstructor_EmptyContent()
    {
        var w = new Word();
        Assert.Equal(string.Empty, w.Content);
        Assert.Equal(0, w.Length);
    }

    [Fact]
    public void Punctuation_Length_And_Value()
    {
        var p = new Punctuation("?");
        Assert.Equal(1, p.Length);
        Assert.Equal("?", p.Value);
    }
}