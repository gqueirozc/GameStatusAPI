using System.Diagnostics.CodeAnalysis;
namespace GameStatusAPI.Factories
{
    public abstract class BaseHttpClient
    {
        protected HttpClient HttpClient;

        [ExcludeFromCodeCoverage]
        protected BaseHttpClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        [ExcludeFromCodeCoverage]
        public virtual async Task<string> GetStringAsync(string url)
        {
            SetupHeaders();
            return await HttpClient.GetStringAsync(url);
        }

        [ExcludeFromCodeCoverage]
        public virtual async Task<HttpResponseMessage> PostAsync(string url, StringContent payload)
        {
            SetupHeaders();
            return await HttpClient.PostAsync(url, payload);
        }

        protected abstract void SetupHeaders();
    }
}
