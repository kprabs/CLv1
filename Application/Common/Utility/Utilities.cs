using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO.Compression;
using System.Reflection;
using System.Text;

namespace CoreLib.Application.Common.Utility
{
    public static class Utilities
    {
        public static string FormatDate(this DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : string.Empty;
        }

        public static DateTime? DateConvertion(string inputDateTime)
        {
            return DateTime.TryParse(inputDateTime, CultureInfo.InvariantCulture, out DateTime result) ? result : null;
        }

        public static string? GetDisplayName(this Enum enumValue)
        {
#pragma warning disable S6608 // Prefer indexing instead of "Enumerable" methods on types implementing "IList"
            if (enumValue != null)
            {
                return enumValue.GetType()
              .GetMember(enumValue.ToString())
              .First()
              .GetCustomAttribute<DisplayAttribute>()
              ?.GetName();
            }

            return string.Empty;
#pragma warning restore S6608 // Prefer indexing instead of "Enumerable" methods on types implementing "IList"
        }

        public static string GZipDecompressString(string zippedString)
        {
            if (string.IsNullOrEmpty(zippedString) || zippedString.Length == 0)
            {
                return string.Empty;
            }
            else
            {
                byte[] zippedData = Convert.FromBase64String(zippedString);
                return System.Text.Encoding.UTF8.GetString(Decompress(zippedData));
            }
        }

        public static string GZipCompressJsonString(string JSONString)
        {
            string outpuString = string.Empty;
            if (!string.IsNullOrEmpty(JSONString))
            {
                byte[] originalData = System.Text.Encoding.UTF8.GetBytes(JSONString);
                using (MemoryStream msi = new(originalData))
                using (MemoryStream mso = new())
                {
                    using (var gs = new GZipStream(mso, CompressionMode.Compress))
                    {
                        CopyTo(msi, gs);
                    }
                    outpuString = Convert.ToBase64String(mso.ToArray());
                }
            }
            else
            {
                outpuString = "Unable to convert this string";
            }
            return outpuString;
        }

        private static byte[] Decompress(byte[] zippedData)
        {
            MemoryStream ms = new(zippedData);
            GZipStream compressedzipStream = new(ms, CompressionMode.Decompress);
            MemoryStream outBuffer = new();
            byte[] block = new byte[1024];
            while (true)
            {
                int bytesRead = compressedzipStream.Read(block, 0, block.Length);
                if (bytesRead <= 0)
                {
                    break;
                }
                else
                {
                    outBuffer.Write(block, 0, bytesRead);
                }
            }
            compressedzipStream.Close();
            return outBuffer.ToArray();
        }


        private static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        public static string QuoteString(string input)
        {
            return !string.IsNullOrEmpty(input) ? input.Replace("'", "''", StringComparison.InvariantCultureIgnoreCase) : "";
        }

        public static string ToDelimiter<T>(this List<T> items, string delimiter)
        {
            return String.Join(delimiter, items.ToArray());
        }

        #region SQL Array utils
        public static string? ToInString(this List<string> items)
        {
            string? result = null;

            if (items?.Count > 0)
            {
                StringBuilder sb = new();
                int loopCount = 0;

                foreach (string item in items)
                {
                    ++loopCount;

                    if (!string.IsNullOrWhiteSpace(sb.ToString()))
                    {
                        string delimiter = loopCount > 1 && loopCount < items.Count ? "," : "";
                        sb.Append($"{delimiter}'{item.Trim()}'");
                    }
                }
                result = sb.ToString();
            }
            return result;
        }

        public static string? ToInString(this List<int> items)
        {
            string? result = null;

            if (items?.Count > 0)
            {
                StringBuilder sb = new();
                int loopCount = 0;

                foreach (int item in items)
                {
                    ++loopCount;

                    if (!string.IsNullOrWhiteSpace(sb.ToString()))
                    {
                        string delimiter = loopCount > 1 && loopCount < items.Count ? "," : "";
                        sb.Append($"{delimiter}'{item.ToString()}'");
                    }
                }
                result = sb.ToString();
            }
            return result;
        }

        public static string? ToInString(this List<long> items)
        {
            string? result = null;

            if (items?.Count > 0)
            {
                StringBuilder sb = new();
                int loopCount = 0;

                foreach (long item in items)
                {
                    ++loopCount;

                    if (!string.IsNullOrWhiteSpace(sb.ToString()))
                    {
                        string delimiter = loopCount > 1 && loopCount < items.Count ? "," : "";
                        sb.Append($"{delimiter}'{item.ToString()}'");
                    }
                }
                result = sb.ToString();
            }
            return result;
        }
        #endregion
    }
}
