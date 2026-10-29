using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

public class TextTests
{
    // ---------- 1. Сортировка по количеству слов ----------
    [Fact]
    public void GetSentencesByWordCountAsc_OrdersCorrectly()
    {
        var text = TextParser.Parse("Mary had a little lamb. Hi. How are you today?");
        var ordered = text.GetSentencesByWordCountAsc().ToList();

        // "Hi." - 1 слово, "How are you today?" - 4 слова, "Mary had a little lamb." - 5 слов
        Assert.Equal(new[] { 1, 4, 5 }, ordered.Select(s => s.WordCount));
    }

    // ---------- 2. Сортировка по длине ----------
    [Fact]
    public void GetSentencesByLengthAsc_OrdersCorrectly()
    {
        var text = TextParser.Parse("A. Aaaa. Aaaaaaa.");
        var ordered = text.GetSentencesByLengthAsc().ToList();
        var lengths = ordered.Select(s => s.Length).ToList();
        Assert.Equal(lengths.OrderBy(x => x), lengths);
    }

    // ---------- 3. Слова заданной длины в вопросительных ----------
    [Fact]
    public void FindWordsOfLengthInInterrogative_FindsOnlyInterrogative_NoDuplicates()
    {
        var text = TextParser.Parse(
            "Did you see her lamb? Is this a test? Statement here.");
        var result = text.FindWordsOfLengthInInterrogative(3);

        // Did(3), you(3), see(3), her(3) - все в первом вопросительном предложении
        // Во втором: Is(2), this(4), a(1), test(4) - нет слов длины 3
        Assert.Equal(new[] { "Did", "her", "see", "you" }, result);
    }

    [Fact]
    public void FindWordsOfLengthInInterrogative_CaseInsensitiveDistinct()
    {
        var text = TextParser.Parse("Cat dog cat?");
        var result = text.FindWordsOfLengthInInterrogative(3);

        // Cat(3), dog(3), cat(3) -> дубликаты без учета регистра -> Cat, dog
        Assert.Equal(new[] { "Cat", "dog" }, result);
    }

    [Fact]
    public void FindWordsOfLengthInInterrogative_EmptyWhenNoMatch()
    {
        var text = TextParser.Parse("Hi. Ok.");
        Assert.Empty(text.FindWordsOfLengthInInterrogative(99));
    }

    // ---------- 4. Удаление слов длины N, начинающихся с согласной ----------
    [Fact]
    public void RemoveWordsOfLengthStartingWithConsonant_RemovesOnlyConsonantStarters()
    {
        var text = TextParser.Parse("Mary had a little lamb.");
        text.RemoveWordsOfLengthStartingWithConsonant(4);

        // Mary(4, M-согл) и lamb(4, l-согл) — удалить; little(6) и had(3) — оставить
        var s = text.Sentences[0];
        Assert.Equal(new[] { "had", "a", "little" },
                     s.Words.Select(w => w.Value));
    }

    // ---------- 5. Замена в конкретном предложении ----------
    [Fact]
    public void ReplaceWordsInSentence_ReplacesOnlyTargetSentence()
    {
        var text = TextParser.Parse("Mary had lamb. Mary was here.");
        text.ReplaceWordsInSentence(0, 4, "XXX");

        // В первом предложении длина 4: Mary, lamb. Оба заменятся на XXX.
        Assert.Equal("XXX had XXX.", text.Sentences[0].ToString());

        // Второе предложение не трогаем (там Mary тоже длины 4, но индекс 1)
        Assert.Equal("Mary was here.", text.Sentences[1].ToString());
    }

    [Fact]
    public void ReplaceWordsInSentence_InvalidIndex_Throws()
    {
        var text = TextParser.Parse("Hi.");
        Assert.Throws<System.ArgumentOutOfRangeException>(
            () => text.ReplaceWordsInSentence(5, 3, "X"));
        Assert.Throws<System.ArgumentOutOfRangeException>(
            () => text.ReplaceWordsInSentence(-1, 3, "X"));
    }

    [Fact]
    public void ReplaceWordsInSentence_ReplacementLengthMayDiffer()
    {
        var text = TextParser.Parse("cat dog.");
        text.ReplaceWordsInSentence(0, 3, "elephant");
        Assert.Equal("elephant elephant.", text.Sentences[0].ToString());
    }

    // ---------- 6. Удаление стоп-слов ----------
    [Fact]
    public void RemoveStopwords_RemovesCaseInsensitive()
    {
        var text = TextParser.Parse("The cat sat on a mat.");
        var stop = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
        { "the", "a", "on" };
        text.RemoveStopwords(stop);
        Assert.Equal("cat sat mat.", text.Sentences[0].ToString());
    }

    [Fact]
    public void RemoveStopwords_NullOrEmptySet_NoChanges()
    {
        var text = TextParser.Parse("The cat.");
        var before = text.ToString();
        text.RemoveStopwords(new HashSet<string>());
        text.RemoveStopwords(null!);
        Assert.Equal(before, text.ToString());
    }

    [Fact]
    public void RemoveStopwords_FromFiles()
    {
        var text = TextParser.Parse("The cat and the dog.");
        var stop = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
        {
            "the", "and"
        };
        text.RemoveStopwords(stop);
        Assert.Equal("cat dog.", text.Sentences[0].ToString());
    }

    // ---------- Вспомогательные ----------
    [Fact]
    public void Text_Words_EnumeratesAllWords()
    {
        var text = TextParser.Parse("One two. Three.");
        Assert.Equal(3, text.Words.Count());
    }

    [Fact]
    public void Text_ToString_JoinsSentencesWithSpace()
    {
        var text = TextParser.Parse("One. Two.");
        Assert.Equal("One. Two.", text.ToString());
    }
}