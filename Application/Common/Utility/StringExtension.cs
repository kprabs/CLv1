namespace CoreLib.Application.Common.Utility
{
    public static class StringExtension
    {
        public static bool EqualsIgnoreCase(this string strA, string strB)
        {
            if (!string.IsNullOrWhiteSpace(strA) && !string.IsNullOrWhiteSpace(strB))
            {
                return strA.Equals(strB, StringComparison.OrdinalIgnoreCase);
            }
            else if (string.IsNullOrWhiteSpace(strA) && string.IsNullOrWhiteSpace(strB))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ContainsIgnoreCase(this string strA, string strB)
        {
            if (string.IsNullOrWhiteSpace(strA) || string.IsNullOrWhiteSpace(strB))
            {
                return true;
            }
            return strA.Contains(strB, StringComparison.OrdinalIgnoreCase);
        }

        public static string GetOrganizationNameByOrgGuid(this string orgguid)
        {
            // we will get organization name base on org guid.
            return orgguid switch
            {
                "32eae766-d432-45cb-ad60-6a6a2ebf0438" => "IBX",
                "3fe964a2-2c64-4363-89d5-9f4d7b2e6108" => "IBX",
                "211c55c7-7806-4117-a3a4-ac30db4af11f" => "AH",
                "15cba54b-7a1e-4de1-92ed-d2cc16fe55b1" => "AH",
                "e17b1e28-c332-4814-ada2-a00b6d750d1d" => "QCC",
                "33b4fe18-2390-4363-8d3f-3b469fad0bb5" => "QCC",
                _ => "IBX",
            };
        }

        public static string ToTrim(this string str)
        {
            str ??= string.Empty;
            var trimChars = " 0";
            str = str.TrimStart(trimChars.ToCharArray());
            return str;
        }

        public static string? GetActualSystemRole(this string userRole)
        {
            return userRole == null ? userRole : userRole.Split("=").LastOrDefault();
        }
        public static string? ToQuoteString(this string input)
        {
            return !string.IsNullOrEmpty(input) ? Utilities.QuoteString(input) : input;
        }
    }
}
