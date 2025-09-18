using CoreLib.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CoreLib.Infrastructure.Persistence
{
    public class AppSettingsProvider(IConfiguration configuration) : IAppSettingsProvider
    {
        public string GetAppSettingKey(string key)
        {
            return configuration[key] ?? String.Empty;
        }

        public string GetAppSettingSectionKey(string section, string key)
        {
            var configSection = configuration.GetSection(section);
            return configSection[key] ?? String.Empty;
        }
    }
}
