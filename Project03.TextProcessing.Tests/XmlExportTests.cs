using System;
using System.IO;
using System.Xml.Serialization;
using Xunit;

public class XmlExportTests
{
    [Fact]
    public void ExportToXml_CreatesFileWithExpectedStructure()
    {
        var text = TextParser.Parse("Hello world. How are you?");
        var path = Path.Combine(Path.GetTempPath(), $"t_{Guid.NewGuid():N}.xml");
        try
        {
            text.ExportToXml(path);
            Assert.True(File.Exists(path));

            var xml = File.ReadAllText(path);
            Assert.Contains("<text", xml);
            Assert.Contains("<sentence>", xml);
            Assert.Contains("<word>", xml);
            Assert.Contains("<punctuation>", xml);
            Assert.Contains("Hello", xml);
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    [Fact]
    public void XmlSerializer_RoundTrip_PreservesContent()
    {
        var original = TextParser.Parse("Mary had a little lamb. Aesop and?");
        var serializer = new XmlSerializer(typeof(Text));

        string xml;
        using (var sw = new StringWriter())
        {
            serializer.Serialize(sw, original);
            xml = sw.ToString();
        }

        Text restored;
        using (var sr = new StringReader(xml))
            restored = (Text)serializer.Deserialize(sr)!;

        Assert.Equal(original.Sentences.Count, restored.Sentences.Count);
        for (int i = 0; i < original.Sentences.Count; i++)
        {
            Assert.Equal(original.Sentences[i].WordCount,
                         restored.Sentences[i].WordCount);
            Assert.Equal(original.Sentences[i].ToString(),
                         restored.Sentences[i].ToString());
        }
    }

    [Fact]
    public void ExportToXml_EmptyText_ProducesValidRoot()
    {
        var text = new Text();
        var path = Path.Combine(Path.GetTempPath(), $"e_{Guid.NewGuid():N}.xml");
        try
        {
            text.ExportToXml(path);
            var xml = File.ReadAllText(path);
            Assert.Contains("<text", xml);
            Assert.DoesNotContain("<sentence>", xml);
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    [Fact]
    public void ExportToXml_ContainsUtf8Declaration()
    {
        var text = TextParser.Parse("Привет мир.");
        var path = Path.Combine(Path.GetTempPath(), $"u_{Guid.NewGuid():N}.xml");
        try
        {
            text.ExportToXml(path);
            var xml = File.ReadAllText(path);
            Assert.Contains("utf-8", xml.ToLowerInvariant());
            Assert.Contains("Привет", xml);
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }
}