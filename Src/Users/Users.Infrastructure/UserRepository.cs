
using Users.Application;
using Users.Domain;
using Microsoft.EntityFrameworkCore;

namespace Users.Infrastructure
{
    public class UserRepository(AppDbContext dbContext, EntityMapper entityMapper) : IUserRepository
    {
        public async Task AddUserAsync(User user, CancellationToken ct = default)
        {
            await dbContext.Users.AddAsync(entityMapper.MapUserToEntity(user), ct);
            await dbContext.SaveChangesAsync();
        }

        public async Task<User?> GetUserAsync(string userName, CancellationToken ct = default)
        {
            var userEntity = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Name == userName, ct);

            if (userEntity == null)
                return null;

            return entityMapper.MapEntityToUser(userEntity);
        }
    }
}
