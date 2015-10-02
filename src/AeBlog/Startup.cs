using AeBlog.Caching;
using Microsoft.AspNet.Builder;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
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
            services.AddAssembly(typeof(Program).GetTypeInfo().Assembly);
            services.AddSingleton(x => MemoryCache.INSTANCE);
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            app.UseStatusCodePagesWithReExecute("/errors/{0}");
            app.UseStaticFiles();
            app.UseErrorPage();
            app.UseMvc();
        }
    }
}
