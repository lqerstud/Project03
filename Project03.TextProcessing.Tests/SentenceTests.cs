using Xunit;

public class SentenceTests
{
    private static Sentence Build(params Token[] tokens) => new(tokens);

    [Fact]
    public void WordCount_And_Length_AreComputed()
    {
        var s = Build(new Word("Mary"), new Word("had"), new Word("lamb"),
                      new Punctuation("."));
        Assert.Equal(3, s.WordCount);
        Assert.Equal(4 + 3 + 4 + 1, s.Length);
    }

    [Theory]
    [InlineData("?", true)]
    [InlineData("!", false)]
    [InlineData(".", false)]
    public void IsInterrogative_DependsOnLastPunctuation(string punct, bool expected)
    {
        var s = Build(new Word("Hi"), new Punctuation(punct));
        Assert.Equal(expected, s.IsInterrogative);
    }

    [Fact]
    public void IsInterrogative_NoPunctuation_False()
    {
        var s = Build(new Word("Hi"));
        Assert.False(s.IsInterrogative);
    }

    [Fact]
    public void RemoveWords_FiltersOutMatching()
    {
        var s = Build(new Word("A"), new Word("cat"), new Word("sat"),
                      new Word("on"), new Word("a"), new Word("mat"),
                      new Punctuation("."));
        s.RemoveWords(w => w.Length == 1);
        Assert.Equal(4, s.WordCount); // cat sat on mat
        Assert.DoesNotContain(s.Words, w => w.Length == 1);
    }

    [Fact]
    public void ReplaceWords_ReplacesMatching()
    {
        var s = Build(new Word("Mary"), new Word("had"), new Word("lamb"),
                      new Punctuation("."));

        s.ReplaceWords(w => w.Length == 3, "###");

        Assert.Contains(s.Words, w => w.Value == "###");
        // Теперь слово длины 3 только одно — это "###"
        Assert.Equal(1, s.Words.Count(w => w.Length == 3));

        // Точнее: Mary(4) и lamb(4) остались, had заменён на ###(3)
        Assert.Equal("Mary ### lamb.", s.ToString());
    }
    [Fact]
    public void ToString_ReconstructsSpacingAndPunctuation()
    {
        var s = Build(new Word("Hello"), new Punctuation(","),
                      new Word("world"), new Punctuation("!"));
        Assert.Equal("Hello, world!", s.ToString());
    }

    [Fact]
    public void ToString_QuoteIsAttached()
    {
        var s = Build(new Punctuation("\""), new Word("Hi"),
                      new Punctuation("\""));
        Assert.Equal("\"Hi\"", s.ToString());
    }

    [Fact]
    public void EmptySentence_PropertiesAreZeroOrFalse()
    {
        var s = new Sentence();
        Assert.Equal(0, s.WordCount);
        Assert.Equal(0, s.Length);
        Assert.False(s.IsInterrogative);
        Assert.Equal(string.Empty, s.ToString());
    }
}