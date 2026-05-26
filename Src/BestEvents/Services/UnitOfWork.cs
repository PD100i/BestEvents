using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BestEvents
{
    /// <summary>
    /// Класс для управления транзакциями и контекстом базы данных
    /// </summary>
    /// <param name="context"></param>
    public class UnitOfWork(AppDbContext context) : IUnitOfWork
    {
        /// <inheritdoc/>
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await context.Database.BeginTransactionAsync();
        }

        /// <inheritdoc/>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            context.Dispose();
        }
    }
}
