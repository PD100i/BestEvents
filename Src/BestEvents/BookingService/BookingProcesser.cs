using BestEvents.Exceptions;
using System.Collections;

namespace BestEvents
{
    /// <summary>
    /// Фоновый сервис для обработки бронирований
    /// </summary>
    public class BookingProcesser(IServiceScopeFactory scopeFactory, ILogger<BookingProcesser> logger) : BackgroundService
    {
        private readonly SemaphoreSlim semaphore = new (1, 1);

        private int pollingDelay = 100;
        private int processingDelay = 2000;

        /// <summary>
        /// Фоновый процесс обработки бронирования
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Task? resultTask = null;
                try
                {
                    using var scope = scopeFactory.CreateScope();
                    var eventRepo = scope.ServiceProvider.GetRequiredService<IEventRepository>();
                    var bookingRepo = scope.ServiceProvider.GetRequiredService<IBookingRepository>();

                    List<Booking> pendingBookings = await bookingRepo.GetPendingBookingAsync(stoppingToken);
                    if (pendingBookings == null || pendingBookings.Count == 0)
                    {
                        await Task.Delay(pollingDelay, stoppingToken);
                        continue;
                    }
                    var tasks = pendingBookings.Select(booking => ProcessBookingAsync(bookingRepo, eventRepo, booking, stoppingToken));
                    resultTask = Task.WhenAll(tasks);
                    await resultTask;
                }
                catch (OperationCanceledException) { throw; }
                catch (Exception ex)
                {
                    if (resultTask != null && resultTask.Exception != null)
                    {
                        foreach (var _ex in resultTask.Exception.InnerExceptions)
                        {
                            logger.LogError(_ex, _ex.Message);
                        }
                    }
                    else
                    {
                        logger.LogError(ex, Messages_ru.UnexpectedBookingError);
                    }
                    await Task.Delay(pollingDelay, stoppingToken);
                }
            }
        }

        private async Task ProcessBookingAsync(IBookingRepository bookingRepository, IEventRepository eventRepository, Booking booking, CancellationToken stoppingToken)
        {
            Event? _event = null;
            try
            {
                stoppingToken.ThrowIfCancellationRequested();
                await Task.Delay(processingDelay, stoppingToken);

                await semaphore.WaitAsync();

                _event = await eventRepository.GetEventAsync(booking.EventId);
                if (_event == null)
                    throw new EventNotFoundException(Messages_ru.EventNotFound);

                if (_event.EndAt < DateTime.Now)
                    throw new EventCompletedException();

                booking.Confirm();
                await bookingRepository.ReplaceBookingAsync(booking, stoppingToken);
            }
            catch (OperationCanceledException)
            { 
                throw; 
            }
            catch
            {
                if (_event!= null)
                {
                    _event.ReleaseSeats();
                    await eventRepository.ReplaceEventAsync(_event);
                }
                booking.Reject();
                await bookingRepository.ReplaceBookingAsync(booking, stoppingToken);
                throw;
            }
            finally
            { 
                semaphore.Release();
            }
        }
    }
}
