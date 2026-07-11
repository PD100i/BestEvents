

namespace Events.Application
{
    /// <summary>
    /// Интерфейс UOF
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Сохраняет изменения в репозитории
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Управляет транзакциями в случае блокировок
        /// </summary>
        /// <returns></returns>
        Task<ITransaction> BeginTransactionAsync();

        /// <summary>
        /// Очищает контекст базы данных
        /// </summary>
        /// <returns></returns>
        void CleanContext();
    }
}
