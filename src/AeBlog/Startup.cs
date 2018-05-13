using AeBlog.Services;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.S3;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace AeBlog
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSingleton<IBlogPostRetriever, BlogPostRetriever>();
            services.AddSingleton<IAmazonDynamoDB>(new AmazonDynamoDBClient(RegionEndpoint.EUWest1));
            services.AddSingleton<IAmazonS3>(new AmazonS3Client(RegionEndpoint.EUWest1));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {
                        options.LoginPath = "/admin/login";
                    })
                    .AddTwitter(options =>
                    {
                        options.ConsumerKey = Environment.GetEnvironmentVariable("TWITTER_CONSUMER_KEY");
                        options.ConsumerSecret = Environment.GetEnvironmentVariable("TWITTER_CONSUMER_SECRET");
                    });

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            loggerFactory.AddLambdaLogger();

            app.UseStaticFiles();
            app.UseStatusCodePagesWithReExecute("/error");
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
        }
    }
}
