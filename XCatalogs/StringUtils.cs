using System.Text;
namespace XCatalogs
{
    internal static class StringUtils
    {
        public static string BytesToHex(byte[] data)
        {
            var sb = new StringBuilder();

            foreach (var item in data)
            {
                sb.Append(item.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}