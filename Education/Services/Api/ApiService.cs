namespace Education.Services.Api
{
    public class ApiService
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

        public async Task PostAsync(string url, MultipartFormDataContent data)
        {
            var response = await _httpClient.PostAsync(url, data);
            response.EnsureSuccessStatusCode();
            await response.Content.ReadAsStringAsync();
        }

        public async Task PutAsync(string url, MultipartFormDataContent data)
        {
            var response = await _httpClient.PutAsync(url, data);
            response.EnsureSuccessStatusCode();
            await response.Content.ReadAsStringAsync();
        }

        public async Task DeleteAsync(string url)
        {
            var response = await _httpClient.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
        }
    }
}

