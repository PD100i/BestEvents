using Users.Domain;

namespace Users.Application
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user, CancellationToken ct = default);

        Task<User?> GetUserAsync(string userName, CancellationToken ct = default);

    }
}
