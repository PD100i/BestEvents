
using BestEvents.Application;
using BestEvents.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;


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
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUserAccessor, UserAccessor>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearerSheme";
            })
            .AddJwtBearer("JwtBearerSheme", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "MyAuthServer",
                    ValidAudience = "MyApiClient",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SUPER_SECRET_KEY_IMPORTANT_MUST_BE_LONG_ENOUGH_12345"))
                };
            });


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
