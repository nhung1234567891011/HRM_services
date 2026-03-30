using HRM_BE.Api.Services;
using HRM_BE.Api.Services.Interfaces;
using HRM_BE.Core.ISeedWorks;
using HRM_BE.Core.IServices;
using HRM_BE.Data.Repositories;
using HRM_BE.Data.SeedWorks;
using HRM_BE.Data.Services;


namespace HRM_BE.Api.Providers
{
    public static class ScopedProvider
    {



        public static IServiceCollection AddScopedProvider(this IServiceCollection services)
        {

            var repositoryAssembly = typeof(BannerRepository).Assembly;
            
            var servicesR = repositoryAssembly.GetTypes()
            .Where(x => x.GetInterfaces().Any(i => i.Name == typeof(Core.ISeedWorks.IRepositoryBase<,>).Name) && !x.IsAbstract && x.IsClass && !x.IsGenericType);

            foreach (var service in servicesR)
            {
                var allInterfaces = service.GetInterfaces();
                var directInterface = allInterfaces.Except(allInterfaces.SelectMany(t => t.GetInterfaces())).FirstOrDefault();
                if (directInterface != null)
                {
                    services.Add(new ServiceDescriptor(directInterface, service, ServiceLifetime.Scoped));
                }
            }

            var dataServices = repositoryAssembly.GetTypes()
                .Where(x => x.Namespace != null 
                    && x.Namespace.Contains("HRM_BE.Data.Services") 
                    && x.IsClass 
                    && !x.IsAbstract)
                .ToList();

            foreach (var serviceImpl in dataServices)
            {
                var interfaceType = serviceImpl.GetInterfaces()
                    .FirstOrDefault(i => i.Namespace != null && i.Namespace.Contains("HRM_BE.Core.IServices"));
                
                if (interfaceType != null)
                {
                    services.AddScoped(interfaceType, serviceImpl);
                }
            }


            services.AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped(typeof(RepositoryBase<,>), typeof(RepositoryBase<,>))
            .AddScoped<ITimesheetCalculationService, TimesheetCalculationService>()




            .AddScoped<IFileService, FileService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IJobHangFireService, JobHangFireService>();
            services.AddScoped<IPermissionService, PermissionService>();

            return services;
        }
    }

}
