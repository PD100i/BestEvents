using Common;
using Events.Application;
using Events.Infrastructure.Consumers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;



namespace Events.Infrastructure
{
    /// <summary>
    ///  Содержит метод расширения для добавления объектов через DI
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Добавляет объекты слоя Infrastructure
        /// </summary>
        /// <param name="services"></param>
        /// <param name="сonfiguration"></param>
        /// <returns></returns>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration сonfiguration)
        {
            services.Configure<KafkaSettings>(сonfiguration.GetSection("Kafka"));

            var connectionString = сonfiguration.GetConnectionString("Default");
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

            services.AddSingleton<SeatsReservedProducer>();
            services.AddSingleton<SeatsReservationErrorProducer>();
            services.AddSingleton<EventFilters>();
            services.AddSingleton<Pagination<EventEntity>>();
            services.AddSingleton<EntityMapper>();

            services.AddHostedService<SeatsReservedPublisher>();
            services.AddHostedService<SeatsReservationErrorPublisher>();
            services.AddHostedService<BookingCreatedConsumer>();
            services.AddHostedService<BookingCancelledConsumer>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IEventRepository, EventRepository>();

            return services;
        }
    }
}
