namespace CoreLib.API.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class CustomTimeoutAttribute(int customTime) : Attribute
    {
        public int CustomTime { get; } = customTime;
    }
}
