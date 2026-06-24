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

            if (string.IsNullOrEmpty(password))
                throw new UserRegisterException(Messages_ru.PasswordIsNotSent);
            if (await CheckIfUserExistsAsync(userName,  ct))
                throw new UserRegisterException(string.Format(Messages_ru.UserAlreadyExists, userName));

            string hash = GetPasswordHashCode(password);
            Guid id = Guid.NewGuid();
            try
            {
                User user = User.CreateUser(id, userName, hash, role);
                await userRepository.AddUserAsync(user, ct);
            }
            catch (Exception ex)
            {
                throw new UserRegisterException(ex.Message);
            }
           
        }

        /// <inheritdoc/>
        public async Task<string> GetTokenAsync(string userName, string password, CancellationToken ct)
        {
            var user = await userRepository.GetUserAsync(userName, ct) ??
                throw new UserNotFoundException(string.Format(Messages_ru.UserNotFound, userName));
            if (user.PasswordHash != GetPasswordHashCode(password))
                throw new CreateTokenException(Messages_ru.WrongPassword);

            var claims = new Dictionary<string, object>
            {
                [JwtRegisteredClaimNames.Name] = user.Name,
                [JwtRegisteredClaimNames.Sub] = user.Id.ToString(),
                ["role"] = user.Role.ToString(),
                [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString(),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"] 
                ?? throw new InvalidOperationException(Messages_ru.SecretKeyNotFound))); 
                
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            SecurityTokenDescriptor descriptor = new()
            {
                Issuer = configuration["JwtSettings:Issuer"],
                Audience = configuration["JwtSettings:Audience"],
                Claims = claims,
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(configuration["JwtSettings:ExpiryInMinutes"]!)),
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

        private async Task<bool> CheckIfUserExistsAsync(string userName, CancellationToken ct)
        {
            return (await userRepository.GetUserAsync(userName, ct)) != null;
            
        }
    }
}
