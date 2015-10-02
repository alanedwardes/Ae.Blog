using AeBlog.Caching;
using AeBlog.Options;
using Microsoft.AspNet.Builder;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.Configuration;
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
            var credentials = new ConfigurationBuilder(environment.ApplicationBasePath)
                .AddJsonFile("credentials.json")
                .AddEnvironmentVariables("AeBlog")
                .Build();

            services.AddMvc();
            services.AddLogging();
            services.AddOptions();
            services.AddAssembly(typeof(Program).GetTypeInfo().Assembly);
            services.AddSingleton(x => MemoryCache.INSTANCE);
            services.Configure<Credentials>(credentials);
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseStatusCodePagesWithReExecute("/errors/{0}");
            app.UseErrorPage();
            app.UseMvc();
        }
    }
}
