using AutoMapper;
using CoreLib.API.Helpers;
using CoreLib.Application.Common;
using CoreLib.Application.Common.APIAuditHelper.InsertAPIAudit;
using CoreLib.Application.Common.APIAuditHelper.PCPServices;
using CoreLib.Application.Common.FeatureConfigurationService;
using CoreLib.Application.Common.MessageService;
using CoreLib.Application.Common.Interfaces;
using CoreLib.Application.Common.Mapper;
using CoreLib.Application.Common.UserAuthService;
using Forgerock.SuperAdmin;
using Forgerock.SuperAdmin.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoreLib.Infrastructure.Persistence
{
    public static class MapAuthorization
    {
        public static void AuthMapper(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<AuthorizationNewCore.Nuget.IAuthorizationManager>(sp =>
            new AuthorizationNewCore.Nuget.AuthorizationManager());

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new CommonProfile());
                mc.AddProfile(new APIAuditProfile());
                mc.AddProfile(new APIAuditPCPProfile());
            });

            services.AddAutoMapper(typeof(APIAuditPCPProfile).Assembly);

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddForgeRockServices();

            services.AddScoped<ICacheHelper>(sp => new UserCacheHelper(new MemoryCache(new MemoryCacheOptions())));
            services.AddScoped<IAuthorizationNugetServices>(sp => new AuthorizationNugetServices(new AuthorizationNewCore.Nuget.AuthorizationManager(), mapper, configuration));
            services.AddScoped<ISqlRepository, SqlRepository>();
            services.AddScoped<IFeatureService, FeatureService>();
            services.AddScoped<IClientInformationService, ClientInformationService>();
            services.AddScoped<IProvisionHandlerService, ProvisionHandlerService>(x =>
                new ProvisionHandlerService(x.GetRequiredService<IFeatureHandlerService>(),
                x.GetRequiredService<ISqlRepository>(), configuration,
                x.GetRequiredService<ILogger<ProvisionHandlerService>>(),
                x.GetRequiredService<IClientInformationService>())
            );

            services.AddScoped<IInsertApiAudit, InsertApiAudit>(sp =>
            {
                return new InsertApiAudit(sp.GetRequiredService <ILogger<IInsertApiAudit>>(), sp.GetRequiredService<IGroupPortalRepository>(), mapper);
            });
            services.AddScoped<IFeatureHandlerService, FeatureHandlerService>(sp => new FeatureHandlerService(configuration, sp.GetRequiredService<ILogger<FeatureHandlerService>>()));

            services.AddScoped<IFeatureConfigurationService>(sp =>
            {
                var sqlRepository = sp.GetRequiredService<ISqlRepository>();
                return new FeatureConfigurationService(sqlRepository);
            });

            services.AddScoped<IMessageService>(sp =>
            {
                var groupPortalRepository = sp.GetRequiredService<IGroupPortalRepository>();
                var sqlRepository = sp.GetRequiredService<ISqlRepository>();
                var _logger = sp.GetRequiredService<ILogger<MessageService>>();

                return new MessageService(sqlRepository, groupPortalRepository, _logger);
            });

            services.AddScoped<IGroupPortalRepository>(sp =>
            {
                var groupportalrepository = sp.GetRequiredService<IRepository<GroupPortalDbContext>>();
                var _logger = sp.GetRequiredService<ILogger<GroupPortalRepository>>();

                return new GroupPortalRepository(groupportalrepository, _logger);
            });

            services.AddScoped<IApiAuditPcpDataHandler, ApiAuditPcpDataHandler>(sp =>
            {
                var groupPortalRepositoryPCP = sp.GetRequiredService<IGroupPortalRepository>();
                var _mapper = sp.GetRequiredService<IMapper>();
                return new ApiAuditPcpDataHandler(
                   groupPortalRepositoryPCP, _mapper);
            });

            services.AddScoped<IAutoHandlerService>(sp =>
            {
                var sqlRepository = sp.GetRequiredService<ISqlRepository>();
                var provisionHandlerService = sp.GetRequiredService<IProvisionHandlerService>();
                var groupPortalRepository = sp.GetRequiredService<IGroupPortalRepository>();
                var clientInformationService = sp.GetRequiredService<IClientInformationService>();
                var logger = sp.GetRequiredService<ILogger<AutoHandlerService>>();

                return new AutoHandlerService(sqlRepository, provisionHandlerService, groupPortalRepository, clientInformationService, logger);
            });
            services.AddScoped<IUserAuthorizationService>(
                sp =>
            {
                var sqlRepository = sp.GetRequiredService<ISqlRepository>();
                var frEndPoints = sp.GetRequiredService<IFREndPoints>();
                var logger = sp.GetRequiredService<ILogger<UserAuthorizationService>>();
                var featureService = sp.GetRequiredService<IFeatureService>();
                var featureHandlerService = sp.GetRequiredService<IFeatureHandlerService>();
                var provisionHandlerService = sp.GetRequiredService<IProvisionHandlerService>();
                var autoHandlerService = sp.GetRequiredService<IAutoHandlerService>();
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();

                return new UserAuthorizationService(mapper, sqlRepository, configuration, frEndPoints, logger, featureService, featureHandlerService, autoHandlerService, provisionHandlerService, httpContextAccessor);
            });
        }
    }
}
