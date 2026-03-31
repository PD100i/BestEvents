
namespace BestEvents
{
    /// <summary>
    /// Фоновый сервис для обработки бронирований
    /// </summary>
    public class BookingProcessor(IServiceScopeFactory scopeFactory, ILogger logger) : BackgroundService
    {
        /// <summary>
        /// Фоновый процесс обработки бронирования
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = scopeFactory.CreateScope();
                    var repository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
                    var tasks = await repository.GetPendingBookingAsync(stoppingToken);
                    if (tasks.Count > 0)
                        tasks.ForEach(async task => await BookingProcessing(repository, task, stoppingToken));  
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message, ex);
                }
            }
        }

        private async Task BookingProcessing(IBookingRepository bookingRepository, Booking booking, CancellationToken stoppingToken)
        {
            await Task.Delay(2000, stoppingToken);
            booking.Confirm();
            await bookingRepository.ReplaceBookingAsync(booking, stoppingToken);
        }
    }
}
