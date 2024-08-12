namespace Pizza_Place_Challenge.Core.Data.Base
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(string id);
        Task AddAsync(T entity);
        Task AddAsync(List<T> entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(string id);
    }
}
