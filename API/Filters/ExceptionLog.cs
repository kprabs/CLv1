using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace CoreLib.API.Filters
{
    internal sealed class ExceptionLog(ExceptionContext context)
    {
        private string? _cachedToString;

        internal static readonly Func<object, Exception?, string> Callback = (state, exception) => ((ExceptionLog)state).ToString();

        public override string ToString()
        {
            if (_cachedToString == null)
            {
                StringBuilder builder = new(2 * 1024);
                builder.Append(context.Exception.ToString());

                _cachedToString = builder.ToString();
            }
            return _cachedToString;
        }
    }
}
