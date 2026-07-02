
using Events.Application;

namespace Events.Infrastructure
{
    /// <summary>
    /// Класс для управления транзакциями и контекстом базы данных
    /// </summary>
    /// <param name="context"></param>
    public class UnitOfWork(AppDbContext context) : IUnitOfWork
    {
        /// <inheritdoc/>
        public async Task<ITransaction> BeginTransactionAsync()
        {
            var transaction = await context.Database.BeginTransactionAsync();
            return new Transaction(transaction);
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
