using BestEvents.ErrorHandler;
using BestEvents.Exceptions;

namespace BestEvents
{
    /// <summary>
    /// Фоновый сервис для обработки бронирований
    /// </summary>
    public class BookingProcessor(IServiceScopeFactory scopeFactory, ILogger<BookingProcessor> logger) : BackgroundService
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
                    // var logger = scope.ServiceProvider.GetRequiredService<ILogger<BookingProcessor>>();

                    List<Booking> queue = await bookingRepo.GetPendingBookingAsync(stoppingToken);
                    if (queue.Count == 0)
                    {
                        await Task.Delay(100, stoppingToken);
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
                    logger.LogError($"Непредвиденная ошибка при работе сервиса бронирования: {ex.Message}");
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
                    logger.LogInformation($"Бронирование {booking.Id} отклонено, так как событие {booking.EventId} уже завершилось");
                    return;
                }
               
                booking.Confirm();
                await bookingRepository.ReplaceBookingAsync(booking, stoppingToken);
            }
            catch (OperationCanceledException) { throw; }
            catch (EventNotFoundException)
            {
                booking.Reject();
                await bookingRepository.ReplaceBookingAsync(booking, stoppingToken);
                logger.LogInformation($"Бронирование {booking.Id} отклонено, так как событие {booking.EventId} не существует или оно было удалено");
            }
            catch (ServiceInvalidOperationException ex)
            {
                logger.LogError($"Ошибка при обработке бронирования {booking.Id}: {ex.Message}");
            }
            
            catch (Exception ex)
            {
                logger.LogError($"Непредвиденная ошибка при обработке бронирования {booking.Id}: {ex.Message}");
            }

        }

        
    }
}
