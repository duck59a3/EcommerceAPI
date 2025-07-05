using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Models;
using MyWebApi.Repository.IRepository;
using System.Linq.Expressions;

namespace MyWebApi.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<AppUser> _users;
        public UserRepository(ApplicationDbContext db) 
        {
            _db = db;
            _users = _db.Set<AppUser>();
        }

        public async Task AddUserAsync(AppUser user)
        {
            await _users.AddAsync(user);
        }

        public Task DeleteUserAsync(AppUser user)
        {
            _users.Remove(user);
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<AppUser>> GetAllUsersAsync()
        {
            return await _users.ToListAsync();
        }

        public async Task<AppUser> GetUserByEmailAsync(string email)
        {
            var user = await _users.FirstOrDefaultAsync(u => u.Email == email);
            return user is null ? null! : user;
        }
        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            var user = await _users.FindAsync(id);
     
            return user is not null ? new AppUser
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Role = user.Role,
            } : null!;
        }

        public Task UpdateUserAsync(AppUser user)
        {
            throw new NotImplementedException();
        }
    }
}
