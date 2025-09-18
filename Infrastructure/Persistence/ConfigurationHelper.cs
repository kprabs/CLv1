using Microsoft.Extensions.Configuration;
#pragma warning disable S2223
namespace CoreLib.Infrastructure.Persistence
{
    public static class ConfigurationHelper
    {
        public static IConfiguration config;
        public static void Initialize(IConfiguration Configuration)
        {
            config = Configuration;
        }
    }
}
