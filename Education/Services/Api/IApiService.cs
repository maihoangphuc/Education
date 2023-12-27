namespace Education.Services.Api
{
    public interface IApiService
    {
        string DefautApiBaseUri { get; }
        string ImageApiBaseUri { get; }

        Task<T> GetAsync<T>(string endpoint);
        Task<T> PostAsync<T>(string endpoint, object data);
        Task PostAsync(string endpoint, List<KeyValuePair<string, string>> parameters);
        Task<T> PutAsync<T>(string endpoint, object data);
        Task DeleteAsync(string endpoint);
    }
}
