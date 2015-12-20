using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;

namespace AeBlog.Options
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationBuilder AddGlobalConfigSources(this IConfigurationBuilder builder)
        {
            return builder.AddJsonFile("config.json")
                .AddEnvironmentVariables("AeBlog");
        }

        public static IServiceCollection AddGlobalConfiguration<TServiceCollection>(this TServiceCollection services, IApplicationEnvironment environment)
            where TServiceCollection : IServiceCollection
        {
            var config = new ConfigurationBuilder()
                .AddGlobalConfigSources()
                .Build();

            services.Configure<Credentials>(config.GetSection(nameof(Credentials)));
            services.Configure<General>(config.GetSection(nameof(General)));

            return services;
        }
    }
}
