using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Linq;
using System.Reflection;

namespace AeBlog
{
    public static class TypeRegistrationServices
    {
        public static IServiceCollection AddDefaultServices(this IServiceCollection services, IServiceProvider defaultProvider)
        {
            var runtimeServices = defaultProvider.GetRequiredService<IRuntimeServices>();

            foreach (var service in runtimeServices.Services)
            {
                services.AddInstance(service, defaultProvider.GetRequiredService(service));
            }

            services.AddInstance(runtimeServices);

            return services;
        }

        public static IServiceCollection AddAssembly(this IServiceCollection services, Assembly assembly)
        {
            var types = assembly.GetTypes();
            var interfaceTypes = types.Where(t => t.GetTypeInfo().IsInterface);
            var classTypes = types.Where(t => t.GetTypeInfo().IsClass);

            foreach (var classType in classTypes)
            {
                var expectedInterface = "I" + classType.Name;

                foreach (var interfaceType in interfaceTypes)
                {
                    if (interfaceType.Name == expectedInterface)
                    {
                        services.AddTransient(interfaceType, classType);
                    }
                }
            }

            return services;
        }
    }
}
