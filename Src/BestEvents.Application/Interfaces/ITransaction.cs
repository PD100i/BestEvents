using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestEvents.Application
{
    public interface ITransaction : IDisposable
    {
        Task CommitAsync(CancellationToken ct);
    }
}
