using System.Security.Cryptography;
using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace XCatalogs
{
    public class XCatalogEntry
    {
        public static readonly Regex CatLineRegex = new Regex(@"(?<path>^.+) +(?<length>\d+) +(?<timestamp>\d+) +(?<checksum>[abcdef0-9]{32})$");

        public string Path {get; set;}
        public DateTime Date {get; set;}
        public byte[] Data {get; set;}

        private byte[] ReadData(Stream dataStream, int neededLength)
        {
            var dataBuffer = new BinaryReader(dataStream).ReadBytes(neededLength);

            if (dataBuffer.Length != neededLength)
            {
                throw new IOException($"Failed to read {neededLength} bytes from the dat stream");
            }

            return dataBuffer;
        }

        public XCatalogEntry(string path, DateTime date, IEnumerable<byte> data)
        {
            Path = path;
            Date = date;
            Data = data.ToArray();
        }

        public XCatalogEntry(string catLine, Stream dataStream)
        {
            catLine = catLine.Trim();

            var lineMatch = CatLineRegex.Match(catLine);

            if (lineMatch == null || !lineMatch.Success)
            {
                throw new ArgumentException($"{catLine} is not a valid line in an X catalog file");
            }

            var lengthGroup = lineMatch.Groups["length"];
            var timestampGroup = lineMatch.Groups["timestamp"];
            var checksumGroup = lineMatch.Groups["checksum"];

            var length = int.Parse(lengthGroup.Value);
            var timestamp = int.Parse(timestampGroup.Value);
            var checksum = checksumGroup.Value;

            var date = DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;

            Path = lineMatch.Groups["path"].Value;
            Date = date;
            Data = ReadData(dataStream, length);
        }

        private string GetDataChecksum()
        {
            if (Data.Length == 0)
            {
                return new string('0', 32);
            }
            else
            {
                string result;
                using (var md5 = MD5.Create())
                {
                    result = StringUtils.BytesToHex(md5.ComputeHash(Data));
                }

                return result;
            }
        }

        public string GetHeaderText()
        {
            using (var md5 = MD5.Create())
            {
                return string.Format(
                    "{0} {1} {2} {3}",
                    Path,
                    Data.Length,
                    ((DateTimeOffset)Date).ToUnixTimeSeconds(),
                    GetDataChecksum()
                );
            }
        }
    }
}
