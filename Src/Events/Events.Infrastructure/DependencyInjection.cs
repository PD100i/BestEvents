using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Events.Application;
using Events.Infrastructure.Consumers;



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
