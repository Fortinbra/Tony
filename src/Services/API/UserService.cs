using Abstractions.Repositories;
using Abstractions.Services;
using Models.Users;

namespace Services.API
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _repository;

        public UserService(IRepository<User> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<User> CreateUser(User user)
        {
            var existingUser = await GetUser(user.DiscordId);
            if (existingUser is null)
            {
                return await _repository.CreateAsync(user).ConfigureAwait(false);
            }
            throw new InvalidOperationException("Cannot create a user twice");
        }

        public Task<User> GetUser(Guid id)
        {
            return _repository.GetAsync(id);
        }

        public async Task<User> GetUser(ulong id)
        {
            return (await _repository.GetAsync(x => x.DiscordId == id)).FirstOrDefault();
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _repository.GetAsync();
        }
    }
}
