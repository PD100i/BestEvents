using Bookings.Application;
using Bookings.Domain;
using Microsoft.IdentityModel.JsonWebTokens;
using Bookings.Domain.Exceptions;

namespace Bookings.Presentation
{
    /// <summary>
    /// Сервис для доступа к пользователю из HttpContext
    /// </summary>
    /// <param name="httpContextAccessor"></param>
    public class UserAccessor(IHttpContextAccessor httpContextAccessor, DtoMapper dtoMapper) : IUserAccessor
    {
        /// <summary>
        /// Предоставляет доступ к пользователю из HttpContext
        /// </summary>
        /// <returns></returns>
        public User GetUser()
        {
            var user = httpContextAccessor.HttpContext?.User;
            if (user == null)
                throw new CreateUserException(Messages_ru.UserNotFound);
            string? id = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            string? name = user.FindFirst(JwtRegisteredClaimNames.PreferredUsername)?.Value;
            string? role = user.FindFirst("role")?.Value;
            try
            {
                User _user = new User(id, name, role);
                return _user;
            }
            catch (Exception ex)
            { 
                throw new CreateUserException(ex.Message);
            }

        }
    }
}
