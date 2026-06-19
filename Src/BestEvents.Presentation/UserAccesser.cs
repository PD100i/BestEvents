using BestEvents.Application;
using BestEvents.Domain;
using Microsoft.IdentityModel.JsonWebTokens;
using BestEvents.Infrastructure;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using BestEvents.Application.Exceptions;

namespace BestEvents.Presentation
{
    public class UserAccesser(IHttpContextAccessor httpContextAccessor) : IUserAccessor
    {
        /// <summary>
        /// Предоставляет доступ к пользователю из HttpContext
        /// </summary>
        /// <returns></returns>
        public User? GetCurrentUser()
        {
            var user = httpContextAccessor.HttpContext?.User;
            if (user == null) 
                return null;
            string? id = user.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            string? name = user.FindFirst("sub")?.Value;
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
