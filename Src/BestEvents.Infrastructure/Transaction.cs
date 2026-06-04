using BestEvents.Application;
using Microsoft.EntityFrameworkCore.Storage;


namespace BestEvents.Infrastructure
{
    public class Transaction(IDbContextTransaction dbContextTransaction) : ITransaction
    {
        public async Task CommitAsync(CancellationToken ct)
        {
            await dbContextTransaction.CommitAsync(ct);
        }

        public void Dispose()
        {
            dbContextTransaction.Dispose();
        }
    }
}
