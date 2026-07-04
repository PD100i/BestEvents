using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public abstract class AbstractPublisher(ILogger<AbstractPublisher> logger) : BackgroundService
    {
        private readonly SemaphoreSlim semaphore = new(1, 1);

        const int PollingDelay = 100;

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
                    await Publiсation(stoppingToken);
                    await Task.Delay(PollingDelay, stoppingToken);
                }
                catch (OperationCanceledException) { throw; }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex.Message);
                    await Task.Delay(PollingDelay, stoppingToken);
                }
            }
        }

        protected abstract Task Publiсation(CancellationToken stoppingToken);
    }
}
