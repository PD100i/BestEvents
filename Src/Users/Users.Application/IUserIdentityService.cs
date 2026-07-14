
namespace Users.Application
{
    public interface IUserIdentityService
    {
        public Task RegistrUserAsync(string userName, string password, string? role, CancellationToken ct);

        public Task<string> GetTokenAsync(string userName, string password, CancellationToken ct);


    }
}
