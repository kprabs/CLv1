namespace CoreLib.Application.Common.Utility
{
    public static class Guard
    {
        [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
        public sealed class ValidatedNotNullAttribute : Attribute { }
        public static void NotNull<T>([ValidatedNotNull] this T value, string paramName) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}
