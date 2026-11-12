using System.Linq;
using Xunit;

public class ConcordanceTests
{
    private static Text PiperText() => TextParser.Parse(
        "Peter Piper picked a peck of pickled peppers. " +
        "A peck of pickled peppers Peter Piper picked. " +
        "If Peter Piper picked a peck of pickled peppers, " +
        "where is the peck that Peter Piper picked?");

    [Fact]
    public void BuildConcordance_FrequencyAndLineNumbers_ForPeterPiper()
    {
        var c = PiperText().BuildConcordance();

        Assert.Equal(4, c["peter"].Frequency);
        Assert.Equal(new[] { 1, 2, 3 }, c["peter"].LineNumbers);

        Assert.Equal(4, c["piper"].Frequency);
        Assert.Equal(new[] { 1, 2, 3 }, c["piper"].LineNumbers);

        Assert.Equal(4, c["picked"].Frequency);
        Assert.Equal(new[] { 1, 2, 3 }, c["picked"].LineNumbers);

        Assert.Equal(4, c["peck"].Frequency);
        Assert.Equal(new[] { 1, 2, 3 }, c["peck"].LineNumbers);

        Assert.Equal(3, c["pickled"].Frequency);
        Assert.Equal(new[] { 1, 2, 3 }, c["pickled"].LineNumbers);

        Assert.Equal(1, c["where"].Frequency);
        Assert.Equal(new[] { 3 }, c["where"].LineNumbers);
    }

    [Fact]
    public void BuildConcordance_DeduplicatesLineNumbers_ButCountsEveryOccurrence()
    {
        var text = TextParser.Parse("cat cat cat. dog.");
        var c = text.BuildConcordance();

        Assert.Equal(3, c["cat"].Frequency);            // три вхождения
        Assert.Equal(new[] { 1 }, c["cat"].LineNumbers); // но одна строка

        Assert.Equal(1, c["dog"].Frequency);
        Assert.Equal(new[] { 2 }, c["dog"].LineNumbers);
    }

    [Fact]
    public void BuildConcordance_IsCaseInsensitive()
    {
        var text = TextParser.Parse("The the THE.");
        var c = text.BuildConcordance();

        Assert.Single(c);                                // одна запись
        Assert.Equal(3, c["the"].Frequency);
        Assert.Equal(new[] { 1 }, c["the"].LineNumbers);
    }

    [Fact]
    public void BuildConcordance_ReturnsKeysInAlphabeticalOrder()
    {
        var c = PiperText().BuildConcordance();
        var words = c.Keys.ToList();
        var expected = words.OrderBy(w => w, System.StringComparer.OrdinalIgnoreCase).ToList();

        Assert.Equal(expected, words);
    }

    [Fact]
    public void BuildConcordance_EmptyText_ReturnsEmpty()
    {
        var c = new Text().BuildConcordance();
        Assert.Empty(c);
    }

    [Fact]
    public void ConcordanceEntry_ToString_MatchesSpecFormat()
    {
        var c = TextParser.Parse("a. b b. a b a.").BuildConcordance();

        Assert.Equal("a. 3: 1 3", c["a"].ToString());
        Assert.Equal("b. 3: 2 3", c["b"].ToString());
    }
}