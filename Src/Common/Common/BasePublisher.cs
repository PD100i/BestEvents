using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace Common
{
    public abstract class BasePublisher(ILogger<BasePublisher> logger) : BackgroundService
    {
        const int PollingDelay = 1000;

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
                }
                catch (OperationCanceledException) { throw; }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex.Message);
                }
                await Task.Delay(PollingDelay, stoppingToken);
            }
        }

        protected abstract Task Publiсation(CancellationToken stoppingToken);
    }
}
