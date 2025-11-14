using CommandLine;
using CsvHelper;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace AddBookmarks
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            var parserResult = CommandLine.Parser.Default.ParseArguments<Options>(args);
            await parserResult.WithParsedAsync(RunOptions);
        }

        static async Task RunOptions(Options opts)
        {
            // TODO(KC): Parse and sanity the command line inputs.

            var bookmarks = await Bookmark.FromFile(opts.BookmarkFile);
            foreach (var b in bookmarks)
            {
                Console.WriteLine(b.ToString());
            }

            // TODO(KC): Create a temporary file for the bookmark data that cpdf is accepting.
            string cpdfBookmarkFile = Path.GetTempFileName();
            var outputLines = bookmarks.Select(b => b.ToString()).ToList();
            await File.WriteAllLinesAsync(cpdfBookmarkFile, outputLines);

            // TODO(KC): Run cpdf.
            var args = $"cpdf -add-bookmarks \"{cpdfBookmarkFile}\" \"{opts.PdfFile}\" -o output.pdf";
            ProcessStartInfo startInfo = new ProcessStartInfo("cpdf", args);
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            using (Process? process = Process.Start(startInfo))
            {
                if (process is not null)
                {
                    await process.WaitForExitAsync();
                    int exitCode = process.ExitCode;
                    if (exitCode != 0)
                    {
                        Console.WriteLine($"[error]: the cpdf command failed (args=\"{args}\")");
                    }
                } else
                {
                    Console.WriteLine($"[error]: failed to start cpdf process with args=\"{args}\"");
                }
            }

            // Attempt to clean up if the temp file is still exist.
            if (Path.Exists(cpdfBookmarkFile))
            {
                File.Delete(cpdfBookmarkFile);
            }
        }
    }

   
}

public class Bookmark
{
    public int Level { get; set; }
    public string Title { get;  set; }
    public int PageNumber { get; set; }

    public string ToString()
    {
        return $"{Level} \"{Title}\" {PageNumber}";
    }

    // FIXME(KC): Use stream instead a unit testing with no actual file IO.
    public static async Task<List<Bookmark>> FromFile(string Path)
    {
        using (var reader = new StreamReader(Path))
        using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var bookmarks = csvReader.GetRecords<Bookmark>().ToList();
            return bookmarks;
        }
    }
}

public class Options
{
    [Value(0, MetaName = "BookmarkFile", HelpText = "A path to the CSV file containing bookmark data", Required = true)]
    public string BookmarkFile { get; set; }

    [Value(1, MetaName = "PdfFile", HelpText = "A path to the source PDF file", Required = true)]
    public string PdfFile { get; set; }
}
