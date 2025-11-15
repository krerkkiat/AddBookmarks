using System.Text;

namespace AddBookmarks.Tests;

public class BookmarkTests
{
    [Test]
    public async Task TestFromFile()
    {
        using (MemoryStream stream = new MemoryStream())
        {
            string content = "Level,Title,PageNumber\n0,Title 1,310";
            byte[] payload = Encoding.UTF8.GetBytes(content);
            stream.Write(payload, 0, payload.Length);
            stream.Seek(0, SeekOrigin.Begin);

            var bookmarks = await Bookmark.FromFile(stream);
            Assert.That(bookmarks.Count, Is.EqualTo(1));
            Assert.That(bookmarks.ElementAt(0).Title, Is.EqualTo("Title 1"));
        }   
    }

    [Test]
    public async Task TestFromFileWithComma()
    {
        using (MemoryStream stream = new MemoryStream())
        {
            string content = "Level,Title,PageNumber\n0,\"Test, an escaped comma.\",310";
            byte[] payload = Encoding.UTF8.GetBytes(content);
            stream.Write(payload, 0, payload.Length);
            stream.Seek(0, SeekOrigin.Begin);

            var bookmarks = await Bookmark.FromFile(stream);
            Assert.That(bookmarks.Count, Is.EqualTo(1));
            Assert.That(bookmarks.ElementAt(0).Title, Is.EqualTo("Test, an escaped comma."));
        }
    }
}
