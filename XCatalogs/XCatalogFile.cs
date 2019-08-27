using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace XCatalogs
{
    public class XCatalogFile
    {
        public List<XCatalogEntry> Entries {get; private set;} = new List<XCatalogEntry>();

        private void InitFromStreams(Stream catStream, Stream datStream)
        {
            var catLines = new StreamReader(catStream).ReadToEnd().Split(new [] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in catLines)
            {
                Entries.Add(new XCatalogEntry(line, datStream));
            }
        }

        private void InitFromFiles(string catPath, string datPath)
        {
            using (var catStream = File.Open(catPath, FileMode.Open))
            {
                using (var datStream = File.Open(datPath, FileMode.Open))
                {
                    InitFromStreams(catStream, datStream);
                }
            }
        }

        public XCatalogFile()
        {

        }

        public XCatalogFile(Stream catStream, Stream datStream)
        {
            InitFromStreams(catStream, datStream);
        }

        public XCatalogFile(string catPath, string datPath)
        {
            InitFromFiles(catPath, datPath);
        }

        public XCatalogFile(string basePath)
        {
            InitFromFiles(basePath + ".cat", basePath + ".dat");
        }

        public void Write(Stream catStream, Stream datStream)
        {
            var catWriter = new StreamWriter(catStream);
            var datWriter = new BinaryWriter(datStream);

            var catLines = Entries.Select(entry => entry.GetHeaderText().Trim());
            foreach (var catLine in catLines)
            {
                catWriter.WriteLine(catLine);
            }
            catWriter.Flush();

            foreach (var entry in Entries)
            {
                datWriter.Write(entry.Data);
            }
            datWriter.Flush();
        }

        public void Write(string catPath, string datPath)
        {
            using (var catStream = File.Open(catPath, FileMode.Create))
            {
                using (var datStream = File.Open(datPath, FileMode.Create))
                {
                    Write(catStream, datStream);
                }
            }
        }

        public void Write(string basePath)
        {
            Write(basePath + ".cat", basePath + ".dat");
        }
    }
}
