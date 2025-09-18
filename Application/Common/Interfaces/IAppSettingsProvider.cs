namespace CoreLib.Application.Common.Interfaces
{
    public interface IAppSettingsProvider
    {
        string GetAppSettingKey(string key);

        string GetAppSettingSectionKey(string section, string key);
    }
}
