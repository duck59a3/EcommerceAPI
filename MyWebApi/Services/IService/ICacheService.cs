namespace MyWebApi.Services.IService
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key) where T : class;
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class;
        Task RemoveAsync(string key);
        Task RemoveAllByAsync(string pattern); //ví dụ xóa all cache có key product_ , 
    }
}
