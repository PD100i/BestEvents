using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Users.Application;




namespace Users.Infrastructure
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

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserIdentityService, UserIdentityService>();

            return services;
        }
    }
}
