
using BestEvents.Application;
using BestEvents.Application.Exceptions;
using BestEvents.Domain;
using Microsoft.EntityFrameworkCore;

namespace BestEvents.Infrastructure
{
    public class UserRepository(AppDbContext dbContext, EntityMapper entityMapper) : IUserRepository
    {
        public async Task AddUserAsync(User user, CancellationToken ct = default)
        {
            await dbContext.Users.AddAsync(entityMapper.MapUserToEntity(user), ct);
        }

        public async Task<User> GetUserAsync(string userName, CancellationToken ct = default)
        {
            var userEntity = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Name == userName, ct);

            if (userEntity == null)
                throw new UserNotFoundException(string.Format(Messages_ru.UserNotFound, userName));

            return entityMapper.MapEntityToUser(userEntity);
        }
    }
}
