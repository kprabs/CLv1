using CoreLib.Application.Common.Interfaces;
using CoreLib.Application.Common.Service;
using CoreLib.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreLib
{
    public static class CoreLibServiceRegistration
    {
        public static IServiceCollection AddCoreLibServices(this IServiceCollection services, IConfiguration configuration)
        {
            ConfigurationHelper.Initialize(configuration);
            services.AddScoped<ISqlRepository, SqlRepository>();
            services.AddScoped<IHeaderReaderService, HeaderReaderService>();
            services.AddScoped<IAuthPayloadService, AuthPayloadService>();
            return services;
        }
    }
}
