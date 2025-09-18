using CoreLib.Application.Common.Interfaces;

#pragma warning disable S3994
namespace CoreLib.Infrastructure.Persistence
{
    public class HttpUtility(IAppSettingsProvider appSettingsProvider) : IHttpUtility
    {
        private string _baseUrl = string.Empty;
        
#pragma warning disable S3995
        public void SetBaseUrl(string baseUrl)
        {
            _baseUrl = new Uri(appSettingsProvider.GetAppSettingSectionKey("EnterpriseBaseUrls", baseUrl)).ToString();
        }

        public string ComposeUrl(string relativeUrl)
        {
            return ComposeUrl(relativeUrl, null);
        }
#pragma warning disable S4005
        public string ComposeUrl(string relativeUrl, string[]? routeParameters)
        {

            UriBuilder uriBuilder = new(_baseUrl)
            {
                Path = relativeUrl
            };

            if (routeParameters != null && routeParameters.Length > 0)
            {
                foreach (var routeParameter in routeParameters)
                {
                    uriBuilder.Path += $"/{Uri.EscapeDataString(routeParameter)}";
                }
            }
#pragma warning restore S3995
            return uriBuilder.ToString();
        }

    }
}
