namespace CoreLib.Application.Common.Utility
{
    public class IcpUtilities
    {
        public static string ConvertDefaultDbNullValue(string? value)
        {
            return value == null || value.Trim() == "~" || value.Trim().EqualsIgnoreCase("AEDW Default") ? string.Empty : value;
        }
    }
}
