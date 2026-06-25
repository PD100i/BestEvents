using BestEvents.Application;
using BestEvents.Domain;
using Microsoft.IdentityModel.JsonWebTokens;
using BestEvents.Application.Exceptions;

namespace BestEvents.Presentation
{
    /// <summary>
    /// Сервис для доступа к пользователю из HttpContext
    /// </summary>
    /// <param name="httpContextAccessor"></param>
    public class UserAccessor(IHttpContextAccessor httpContextAccessor) : IUserAccessor
    {
        /// <summary>
        /// Предоставляет доступ к пользователю из HttpContext
        /// </summary>
        /// <returns></returns>
        public User GetUser()
        {
            var user = httpContextAccessor.HttpContext?.User;
            if (user == null)
                throw new GetUserFromRequestException(Messages_ru.UserNotFound);
            string? id = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            string? name = user.FindFirst(JwtRegisteredClaimNames.PreferredUsername)?.Value;
            string? role = user.FindFirst("role")?.Value;
            try
            {
                User _user = User.CreateUser(id, name, role);
                return _user;
            }
            catch (Exception ex)
            { 
                throw new GetUserFromRequestException(ex.Message);
            }

        }
    }
}
