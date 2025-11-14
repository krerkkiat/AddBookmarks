namespace AddBookmarks.Tests;

public class BookmarkTests
{
    private string _csvWithCommaFilePath;
    private string _csvWithoutCommaFilePath;

    [SetUp]
    public void SetUp()
    {
        _csvWithCommaFilePath = Path.GetTempFileName();
        using (var writer = new StreamWriter(_csvWithCommaFilePath))
        {
            writer.WriteLine("Level,Title,PageNumber");
            writer.WriteLine("0,\"Test, an escaped comma.\",310");
        }

        _csvWithoutCommaFilePath = Path.GetTempFileName();
        using (var writer = new StreamWriter(_csvWithoutCommaFilePath))
        {
            writer.WriteLine("Level,Title,PageNumber");
            writer.WriteLine("0,Title 1,310");
        }
    }

    [TearDown]
    public void TearDown()
    {
        if (Path.Exists(_csvWithCommaFilePath))
        {
            File.Delete(_csvWithCommaFilePath);
        }
        if (Path.Exists(_csvWithoutCommaFilePath))
        {
            File.Delete(_csvWithoutCommaFilePath);
        }
    }

    [Test]
    public async Task TestFromFile()
    {
        var bookmarks = await Bookmark.FromFile(_csvWithoutCommaFilePath);
        Assert.That(bookmarks.Count, Is.EqualTo(1));
        Assert.That(bookmarks.ElementAt(0).Title, Is.EqualTo("Title 1"));
    }

    [Test]
    public async Task TestFromFileWithComma()
    {
        var bookmarks = await Bookmark.FromFile(_csvWithCommaFilePath);
        Assert.That(bookmarks.Count, Is.EqualTo(1));
        Assert.That(bookmarks.ElementAt(0).Title, Is.EqualTo("Test, an escaped comma."));
    }
}
