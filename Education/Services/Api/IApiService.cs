namespace Education.Services.Api
{
    public interface IApiService
    {
        string DefautApiBaseUri { get; }
        string ImageApiBaseUri { get; }

        Task<T> GetAsync<T>(string endpoint);
        Task PostAsync(string endpoint, List<KeyValuePair<string, string>> data);
        Task PutAsync(string endpoint, MultipartFormDataContent content);
        Task DeleteAsync(string endpoint);
    }
}
