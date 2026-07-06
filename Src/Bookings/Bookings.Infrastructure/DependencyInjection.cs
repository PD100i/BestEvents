using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Bookings.Application;



namespace Bookings.Infrastructure
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
            
            services.AddSingleton<EntityMapper>();
            services.AddSingleton<BookingCreatedProducer>();
            services.AddSingleton<BookingCancelledProducer>();

            services.AddHostedService<BookingCreatedPublisher>();
            services.AddHostedService<BookingCancelledPublisher>();
            services.AddHostedService<BookingConfirmedConsumer>();
            services.AddHostedService<BookingRejectedConsumer>();
           

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IBookingRepository, BookingRepository>();

            return services;
        }
    }
}
