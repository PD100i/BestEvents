using BestEvents.Application;
using BestEvents.Domain;
using BestEvents.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestEventsIntegrationTest
{
    [Collection("Database collection")]
    public class UserRepositoryTest
    {
        private readonly DatabaseFixture _fixture;

        public UserRepositoryTest(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        private AppDbContext CreateContext()
        {
            return _fixture.CreateContext();
        }

        private async Task InitializeDatabaseAsync()
        {
            await _fixture.ResetDatabaseAsync();
        }

        private UserRepository CreateUserRepository(AppDbContext dbContext)
        {
            return new UserRepository(dbContext, new EntityMapper());
        }

        private async Task AddSomeUsers(AppDbContext dbContext)
        {
            List<UserEntity> someUsers = Enumerable.Range(0, 10).Select((_ , i) => new UserEntity()
            { 
                Id = Guid.NewGuid(),
                Name = $"user_{i}",
                PasswordHash = $"Password_{i}",
                Role = UserRolesEnum.User
            }).ToList();
            await dbContext.Users.AddRangeAsync(someUsers);
        }

        [Fact]
        public async Task AddUserAsync_CorrectData_ShouldAddUser()
        {
            // Arrange
            await InitializeDatabaseAsync();
            var context = CreateContext();
            var user = User.CreateUser(Guid.NewGuid(), "user", "Password", "");
            var repo = CreateUserRepository(context);

            // Act
            await repo.AddUserAsync(user, CancellationToken.None);

            // Assert
            var assertContext = CreateContext();
            UserEntity? userEntity = assertContext.Users.FirstOrDefault(u => u.Id == user.Id);
            Assert.NotNull(userEntity);
            Assert.Equal(userEntity.Name, user.Name);
            Assert.Equal(userEntity.PasswordHash, user.PasswordHash);
            Assert.Equal(userEntity.Role, user.Role);

        }

        [Fact]
        public async Task GetUserAsync_UserExsists_ReturnUser()
        {
            // Arrange
            await InitializeDatabaseAsync();
            var arrangeContext = CreateContext();
            UserEntity entity = new()
            {
                Id = Guid.NewGuid(),
                Name = "user",
                PasswordHash = "password",
                Role = UserRolesEnum.User,
            };
            arrangeContext.Users.Add(entity);
            await AddSomeUsers(arrangeContext);
            await arrangeContext.SaveChangesAsync(CancellationToken.None);

            var context = CreateContext();
            var repo = CreateUserRepository(context);

            // Act
            var user = await repo.GetUserAsync(entity.Name, CancellationToken.None);

            Assert.NotNull(user);
            Assert.Equal(user.Id, entity.Id);
            Assert.Equal(user.Name, entity.Name);
            Assert.Equal(user.PasswordHash, entity.PasswordHash);
            Assert.Equal(user.Role, entity.Role);
        }

        [Fact]
        public async Task GetUserAsync_UserNotFound_ReturnNull()
        {
            // Arrange
            await InitializeDatabaseAsync();
            var arrangeContext = CreateContext();
            string userName = "user";
            await AddSomeUsers(arrangeContext);
            await arrangeContext.SaveChangesAsync(CancellationToken.None);

            var context = CreateContext();
            var repo = CreateUserRepository(context);

            // Act
            var user = await repo.GetUserAsync(userName, CancellationToken.None);

            Assert.Null(user);
            
        }
    }
}
