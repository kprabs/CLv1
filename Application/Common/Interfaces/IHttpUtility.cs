namespace CoreLib.Application.Common.Interfaces
{
#pragma warning disable S3994
#pragma warning disable S3995
    public interface IHttpUtility
    {
        string ComposeUrl(string relativeUrl);
        string ComposeUrl(string relativeUrl, string[] routeParameters);
        void SetBaseUrl(string baseUrl);
    }
#pragma warning restore S3995
    public interface IHttpClientWrapper<T> where T : class
    {
        public Task<T> GetAsync<T1>(string url, T1 queryObject);
        public Task<T> PostAsync(string url, T postObject);
        public Task<T> PostAsync<T1>(string url, T1 postObject);
        public Task PutAsync(string url, T putObject);
    }
}
