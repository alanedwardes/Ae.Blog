using AeBlog.Caching;
using AeBlog.Logging;
using AeBlog.Options;
using Microsoft.AspNet.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using System.Reflection;

namespace AeBlog
{
    public class Startup
    {
        private readonly IApplicationEnvironment environment;

        public Startup(IApplicationEnvironment environment)
        {
            this.environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddLogging();
            services.AddAssembly(typeof(Startup).GetTypeInfo().Assembly);
            services.AddSingleton(x => MemoryCache.INSTANCE);
            services.AddGlobalConfiguration(environment);
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory logger)
        {
            logger.AddConsole();
            logger.AddProvider(app.ApplicationServices.GetService<ISNSLoggerProvider>());
            app.UseStatusCodePagesWithReExecute("/errors/{0}");
            app.UseStaticFiles();
            app.UseExceptionHandler("/errors/500");
            app.UseMvc();
        }
    }
}
