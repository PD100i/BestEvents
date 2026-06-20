using BestEvents.Domain;

namespace BestEvents.Application
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user, CancellationToken ct = default);

        Task<User> GetUserAsync(string userName, CancellationToken ct = default);

        Task<User> GetUserAsync(Guid userId, CancellationToken ct = default);
    }
}
