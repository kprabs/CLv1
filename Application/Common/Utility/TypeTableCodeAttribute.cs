namespace CoreLib.Application.Common.Utility
{
    [AttributeUsage(AttributeTargets.All)]
    public class TypeTableCodeAttribute : Attribute
    {
        public string? Code { get; set; }
        public string? DisplayName { get; set; }

        public TypeTableCodeAttribute(string code)
        {
            this.Code = code;
        }

        public TypeTableCodeAttribute(string code, string displayName)
        {
            this.Code = code;
            this.DisplayName = displayName;
        }

    }
}
