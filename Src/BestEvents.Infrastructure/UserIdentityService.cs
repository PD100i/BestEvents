using BestEvents.Application;
using BestEvents.Application.Exceptions;
using BestEvents.Domain;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;


namespace BestEvents.Infrastructure
{
    public class UserIdentityService(IConfiguration configuration, IUserRepository userRepository) : IUserIdentityService
    {
        /// <inheritdoc/>
        public async Task RegistrUserAsync(string userName, string password, string? role, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            string hash = GetPasswordHashCode(password);
            Guid id = Guid.NewGuid();
            UserEntity user = new(id, userName, hash, role);
            await userRepository.AddUserAsync(user, ct);
        }

        public async Task<string> GetTokenAsync(string userName, string password, CancellationToken ct)
        {
            var user = await userRepository.GetUserAsync(userName, ct) ??
                throw new CreateTokenException(string.Format(Messages_ru.UserNotFound, userName));
            if (user.PasswordHash != GetPasswordHashCode(password))
                throw new CreateTokenException(Messages_ru.WrongPassword);

            var claims = new Dictionary<string, object>
            {
                [JwtRegisteredClaimNames.Name] = user.Name,
                [JwtRegisteredClaimNames.Sub] = user.Id.ToString(),
                ["role"] = user.Role,
                [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString(),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            SecurityTokenDescriptor descriptor = new()
            {
                Issuer = configuration["Jwt:Issuer"],
                Audience = configuration["Jwt:Audience"],
                Claims = claims,
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(15),
                IssuedAt = DateTime.UtcNow,
                SigningCredentials = creds
            };

            return new JsonWebTokenHandler().CreateToken(descriptor);
        }

        private static string GetPasswordHashCode(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes);
        }
    }
}
