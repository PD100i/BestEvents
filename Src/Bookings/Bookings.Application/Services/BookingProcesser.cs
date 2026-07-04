using Bookings.Application.Exceptions;
using System.Collections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bookings.Application
{
    /// <summary>
    /// Фоновый сервис для обработки бронирований
    /// </summary>
    public class BookingProcesser(IServiceScopeFactory scopeFactory, ILogger<BookingProcesser> logger) : BackgroundService
    {
        private readonly SemaphoreSlim semaphore = new(1, 1);

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
                    var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();


                    List<Guid> pendingBookings = bookingService.GetPendingBookings();
                    if (pendingBookings == null || pendingBookings.Count == 0)
                    {
                        await Task.Delay(pollingDelay, stoppingToken);
                        continue;
                    }
                    var tasks = pendingBookings.Select(bookingId => TryBooking(bookingService, bookingId, stoppingToken));
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

        private async Task TryBooking(IBookingService bookingService, Guid id, CancellationToken stoppingToken)
        {
            await Task.Delay(processingDelay);
            await bookingService.TryProcessBooking(id, stoppingToken);
        }
    }   
}
