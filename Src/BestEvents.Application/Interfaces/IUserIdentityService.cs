using BestEvents.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestEvents.Application
{
    public interface IUserIdentityService
    {
        public Task RegistrUserAsync(string userName, string password, string? role, CancellationToken ct);

        public Task<string> GetTokenAsync(string userName, string password, CancellationToken ct);


    }
}
