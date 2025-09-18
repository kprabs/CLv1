namespace CoreLib.Application.Common.Utility
{
    public class ApiTimmer
    {
        public TimeSpan getTimeSpan(int timeoutValue)
        {
            return TimeSpan.FromSeconds(timeoutValue);
        }
    }
}
