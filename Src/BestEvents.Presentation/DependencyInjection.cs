
using BestEvents.Application;
using BestEvents.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Reflection;


namespace BestEvents.Presentation
{
    /// <summary>
    ///  Содержит метод расширения для добавления объектов через DI
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Добавляет объекты слоя Presentation
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddPresentation(this IServiceCollection services)
        {
            services.AddScoped<IUserAccessor, UserAccessor>();

            services.AddControllers(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;
            });

            

            services.AddSingleton<DtoMapper>();

            services.AddSwaggerGen(options =>
            {
                // Путь к XML-файлу с документацией
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });

            return services;
        }
    }
}
