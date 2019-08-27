using System.Text;
using System;

namespace XCatalogs.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var file = new XCatalogFile();

            file.Entries.Add(new XCatalogEntry("foo/bar.txt", DateTime.Now, Encoding.Default.GetBytes("Hello World!")));
            file.Entries.Add(new XCatalogEntry("foo/baz.txt", DateTime.Now.AddDays(-5), Encoding.Default.GetBytes("Hello Teapot!")));
            file.Entries.Add(new XCatalogEntry("foo/empty.txt", DateTime.Now.AddDays(-5), Encoding.Default.GetBytes("")));

            file.Write("testData/ext_01");
        }
    }
}
