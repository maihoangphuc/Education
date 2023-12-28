/*namespace Education.Services.Api
{
    public class ApiService : IApiService
    {
        public string DefautApiBaseUri { get; }
        public string ImageApiBaseUri { get; }

        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient, ApiConfig apiConfig)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");

            DefautApiBaseUri = apiConfig.DefautApiBaseUri;
            ImageApiBaseUri = apiConfig.ImageApiBaseUri;
        }

        public async Task<T> GetAsync<T>(string url)
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<T>();
        }

        public async Task PostAsync(string url, object data)
        {
            var response = await _httpClient.PostAsJsonAsync(url, data);
            response.EnsureSuccessStatusCode();
            await response.Content.ReadAsStringAsync();
        }

        public async Task PostAsync(string url, List<KeyValuePair<string, string>> data)
        {
            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = HttpMethod.Post;
            httpRequestMessage.RequestUri = new Uri(url);

            var content = new FormUrlEncodedContent(data);
            httpRequestMessage.Content = content;

            // Thực hiện Post
            var response = await _httpClient.SendAsync(httpRequestMessage);
            await response.Content.ReadAsStringAsync();
        }

        public async Task PutAsync(string url, MultipartFormDataContent content)
        {
            *//*   var httpRequestMessage = new HttpRequestMessage();
               httpRequestMessage.Method = HttpMethod.Post;
               httpRequestMessage.Headers.Add("User-Agent", "Mozilla/5.0");
               httpRequestMessage.RequestUri = new Uri(url);

               httpRequestMessage.Content = content;

               // Thực hiện Put
               var response = await _httpClient.SendAsync(httpRequestMessage);
               await response.Content.ReadAsStringAsync();*//*

            await _httpClient.PostAsync(url, content);
        }

        public Task DeleteAsync(string url)
        {
            _httpClient.BaseAddress = new Uri(url);

            // Thực hiện Delete
            var response = _httpClient.DeleteAsync(url);
            return response;
        }
    }
}*/