using BestEvents.Exceptions;

namespace BestEvents
{
    /// <summary>
    /// Фоновый сервис для обработки бронирований
    /// </summary>
    public class BookingProcesser(IServiceScopeFactory scopeFactory, ILogger<BookingProcesser> logger) : BackgroundService
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
                    var eventRepo = scope.ServiceProvider.GetRequiredService<IEventRepository>();
                    var bookingRepo = scope.ServiceProvider.GetRequiredService<IBookingRepository>();

                    List<Booking> queue = await bookingRepo.GetPendingBookingAsync(stoppingToken);
                    if (queue == null || queue.Count == 0)
                    {
                        await Task.Delay(500, stoppingToken);
                        continue;
                    }
                    foreach(var task in queue)
                    {
                        await BookingProcessing(bookingRepo, eventRepo, task, stoppingToken);
                    }
                }
                catch (OperationCanceledException) { throw; }
                catch (Exception ex)
                {
                    logger.LogError(ex, Messages_ru.UnexpectedBookingError);
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }

        private async Task BookingProcessing(IBookingRepository bookingRepository, IEventRepository eventRepository, Booking booking, CancellationToken stoppingToken)
        {
            try
            {
                stoppingToken.ThrowIfCancellationRequested();
                await Task.Delay(2000, stoppingToken);

                var _event = eventRepository.GetEvent(booking.EventId);
                if (_event.EndAt < DateTime.Now)
                {
                    booking.Reject();
                    await bookingRepository.ReplaceBookingAsync(booking, stoppingToken);
                    logger.LogInformation(string.Format(Messages_ru.BookingRejectedEventCompleted, booking.Id, booking.EventId));
                    return;
                }
               
                booking.Confirm();
                await bookingRepository.ReplaceBookingAsync(booking, stoppingToken);
            }
            catch (OperationCanceledException) 
                { throw; }
            catch (EventNotFoundException)
            {
                booking.Reject();
                await bookingRepository.ReplaceBookingAsync(booking, stoppingToken);
                logger.LogInformation(string.Format(Messages_ru.BookingRejectedEventNoExist, booking.Id, booking.EventId));
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format(Messages_ru.BookingProcessingError, booking.Id, ex.Message));
            }
        }
    }
}
