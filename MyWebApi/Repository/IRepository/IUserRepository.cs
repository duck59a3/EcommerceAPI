using MyWebApi.Models;

namespace MyWebApi.Repository.IRepository
{
    public interface IUserRepository 
    {
        // Define methods specific to user repository here, if needed
        // For example, you might want to add methods like GetUserByEmail, etc.
        Task<IEnumerable<AppUser>> GetAllUsersAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task AddUserAsync(AppUser user);
        Task UpdateUserAsync(AppUser user);
        Task DeleteUserAsync(AppUser user);
        Task<AppUser> GetUserByEmailAsync(string email);
    }
}
