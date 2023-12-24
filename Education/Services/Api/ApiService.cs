namespace Education.Services.Api
{
    public class ApiService : IApiService
    {
        public string DefautApiBaseUri { get; }
        public string ImageApiBaseUri { get; }

        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient, ApiConfig apiConfig)
        {
            _httpClient = httpClient;

            DefautApiBaseUri = apiConfig.DefautApiBaseUri;
            ImageApiBaseUri = apiConfig.ImageApiBaseUri;
        }

        public async Task<T> GetAsync<T>(string url)
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<T>();
        }

        public async Task<T> PostAsync<T>(string url, object data)
        {
            var response = await _httpClient.PostAsJsonAsync(url, data);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<T>();
        }

        public async Task<T> PutAsync<T>(string url, object data)
        {
            var response = await _httpClient.PutAsJsonAsync(url, data);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<T>();
        }

        public async Task DeleteAsync(string url)
        {
            var response = await _httpClient.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
        }
    }
}