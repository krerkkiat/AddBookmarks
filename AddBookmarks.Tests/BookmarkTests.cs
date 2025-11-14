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
            writer.WriteLine("level,title,page");
            writer.WriteLine("0,\"Test, an escaped comma.\",310");
        }

        _csvWithoutCommaFilePath = Path.GetTempFileName();
        using (var writer = new StreamWriter(_csvWithoutCommaFilePath))
        {
            writer.WriteLine("level,title,page");
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
    }

    [Test]
    [Ignore("Need to update the implementation.")]
    public async Task TestFromFileWithComma()
    {
        var bookmarks = await Bookmark.FromFile(_csvWithCommaFilePath);
        Assert.That(bookmarks.Count, Is.EqualTo(1)); 
    }
}
