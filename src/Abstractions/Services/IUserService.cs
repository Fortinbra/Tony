using Models.Users;

namespace Abstractions.Services
{
    public interface IUserService
    {
        Task<User> GetUser(Guid id);
        Task<IEnumerable<User>> GetUsers();
        Task<User?> GetUser(ulong id);
        Task<User> CreateUser(User user);
    }
}
