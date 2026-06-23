using BestEvents.Application;
using BestEvents.Application.Exceptions;
using BestEvents.Domain;
using BestEvents.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Moq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BestEventsTest
{
    public class UserIdentityServiceTest
    {
        public class UserIdentityServiceFixture
        {
            public Dictionary<string, string?> InMemorySettings { get; }
            public UserIdentityService UserIdentityService { get; }
            public Mock<IUserRepository> MockUserRepo { get; }

            public UserIdentityServiceFixture() 
            {
                InMemorySettings = new Dictionary<string, string?>
                {
                    { "JwtSettings:SecretKey", "J1l/sf2XSvYJZiYXkyf2HZQGDXyMNylggIO/FiWJEyJH3e1ByMhyrxSXhgk4EBjoJjdqbHtoGSaqlaDzceyHpA=="},
                    { "JwtSettings:Issuer", "BestEvents" },
                    { "JwtSettings:Audience", "BestEventsApi" },
                    { "JwtSettings:ExpiryInMinutes", "15" }
                };
                IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(InMemorySettings)
                .Build();
                MockUserRepo = new Mock<IUserRepository>();
                UserIdentityService = new UserIdentityService(configuration, MockUserRepo.Object);
            }
        }

        [Theory]
        [InlineData("Admin", UserRolesEnum.Admin)]
        [InlineData("User", UserRolesEnum.User)]
        [InlineData("", UserRolesEnum.User)]
        [InlineData(null, UserRolesEnum.User)]
        public async Task RegisterUserAsync_CorrectLoginAndPasswordAndRole_ShouldCallRepositoryMethod(string? role, UserRolesEnum expectedRole)
        {
            // Arrange
            var fixture = new UserIdentityServiceFixture();
            string userName = "user";
            string password = "password";

            // Act
            await fixture.UserIdentityService.RegistrUserAsync(userName, password, role, CancellationToken.None);

            // Assert
            fixture.MockUserRepo.Verify(repo => repo.AddUserAsync(It.Is<BestEvents.Domain.User>(u => u.Id != Guid.Empty &&
                                                                                   u.Name == userName &&
                                                                                   u.PasswordHash != password &&
                                                                                   u.Role == expectedRole), It.IsAny<CancellationToken>()));
        }

        [Theory]
        [InlineData("")]
        [InlineData("АБвгд")]
        public async Task RegisterUserAsync_NotCorrectUserName_ShouldThrowRegisterException(string userName)
        {
            // Arrange
            var fixture = new UserIdentityServiceFixture();
            string password = "password";
            string role = "";


            // Act && Assert
            await Assert.ThrowsAsync<UserRegisterException>(() => fixture.UserIdentityService.RegistrUserAsync(userName, password, role, CancellationToken.None));
        }

        [Fact]
        public async Task RegisterUserAsync_NotCorrectPassword_ShouldThrowRegisterException()
        {
            // Arrange
            var fixture = new UserIdentityServiceFixture();
            string userName = "user";
            string password = "";
            string role = "";


            // Act && Assert
            await Assert.ThrowsAsync<UserRegisterException>(() => fixture.UserIdentityService.RegistrUserAsync(userName, password, role, CancellationToken.None));
        }

        [Fact]
        public async Task RegisterUserAsync_NotCorrectRole_ShouldThrowRegisterException()
        {
            // Arrange
            var fixture = new UserIdentityServiceFixture();
            string userName = "user";
            string password = "";
            string role = "Superuser";


            // Act && Assert
            await Assert.ThrowsAsync<UserRegisterException>(() => fixture.UserIdentityService.RegistrUserAsync(userName, password, role, CancellationToken.None));
        }

        [Fact]
        public async Task GetTokenAsync_CorrectData_ShouldReturnValidToken()
        {
            // Arrange
            var fixture = new UserIdentityServiceFixture();
            Guid userId = Guid.NewGuid();
            string userName = "user";
            string password = "password";
            string passwordHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
            string role = "Admin";

            var user = BestEvents.Domain.User.CreateUser(userId, userName, passwordHash, role);

            fixture.MockUserRepo.Setup(repo => repo.GetUserAsync(userName, It.IsAny<CancellationToken>())).ReturnsAsync(user);

            // Act
            string token = await fixture.UserIdentityService.GetTokenAsync(userName, password, CancellationToken.None);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);
            Assert.Equal(3, token.Split(".").Length);
        }

        [Fact]
        public async Task GetTokenAsync_CorrectData_ShoulContentCorrectClaims()
        {
            // Arrange
            var fixture = new UserIdentityServiceFixture();
            Guid userId = Guid.NewGuid();
            string userName = "user";
            string password = "password";
            string passwordHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
            string role = "Admin";
            string expectedIssuer = fixture.InMemorySettings["JwtSettings:Issuer"]!;
            string expectedAudience = fixture.InMemorySettings["JwtSettings:Audience"]!;
            int expectedExpiryInMinutes = int.Parse(fixture.InMemorySettings["JwtSettings:ExpiryInMinutes"]!);
            var expectedExpiration = DateTime.UtcNow.AddMinutes(expectedExpiryInMinutes);

            var user = BestEvents.Domain.User.CreateUser(userId, userName, passwordHash, role);

            fixture.MockUserRepo.Setup(repo => repo.GetUserAsync(userName, It.IsAny<CancellationToken>())).ReturnsAsync(user);

            // Act
            string token = await fixture.UserIdentityService.GetTokenAsync(userName, password, CancellationToken.None);


            // Assert
            var tokenHandler = new JsonWebTokenHandler();
            var jwtToken = tokenHandler.ReadJsonWebToken(token);
            Assert.True(jwtToken?.Audiences.Contains(expectedAudience));
            Assert.Equal(jwtToken?.Issuer, expectedIssuer);

            Assert.NotNull(jwtToken);
            jwtToken.TryGetClaim(JwtRegisteredClaimNames.Name, out Claim _name);
            jwtToken.TryGetClaim(JwtRegisteredClaimNames.Sub, out Claim _sub);
            jwtToken.TryGetClaim("role", out Claim _role);

            Assert.Equal(userName, _name.ToString());
            Assert.Equal(userId.ToString(), _sub.ToString());
            Assert.Equal(role, _role.ToString());
            
            Assert.True((jwtToken.ValidTo - expectedExpiration).Duration() < TimeSpan.FromMinutes(1));
        }

        [Fact]
        public async Task GetTokenAsync_WrongPassword_ThrowUserNotFound()
        {
            // Arrange
            var fixture = new UserIdentityServiceFixture();
            string userName = "user";
            string password = "password";
            string wrongPassword = "wrongPassword";
            string passwordHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
            string role = "Admin";
            Guid userId = Guid.NewGuid();
            var user = BestEvents.Domain.User.CreateUser(userId, userName, passwordHash, role);

            fixture.MockUserRepo.Setup(repo => repo.GetUserAsync(userName, It.IsAny<CancellationToken>())).ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<CreateTokenException>(() => fixture.UserIdentityService.GetTokenAsync(userName, wrongPassword, CancellationToken.None));
        }
    }
}
