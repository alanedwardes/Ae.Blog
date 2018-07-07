using AeBlog.Services;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.S3;
using AspNetCore.DataProtection.Aws.S3;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
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
            services.AddSingleton<IBlogPostRepository, BlogPostRepository>();
            services.AddSingleton<IAmazonDynamoDB>(new AmazonDynamoDBClient(RegionEndpoint.EUWest1));
            services.AddSingleton<IAmazonS3>(new AmazonS3Client(RegionEndpoint.EUWest1));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {
                        options.LoginPath = "/session/login";
                        options.AccessDeniedPath = "/session/denied";
                    })
                    .AddTwitter(options =>
                    {
                        options.ConsumerKey = Environment.GetEnvironmentVariable("TWITTER_CONSUMER_KEY");
                        options.ConsumerSecret = Environment.GetEnvironmentVariable("TWITTER_CONSUMER_SECRET");
                    });

            services.AddAuthorization(options =>
            {
                var isAlanPolicy = new AuthorizationPolicyBuilder(new[] { TwitterDefaults.AuthenticationScheme })
                    .RequireClaim("urn:twitter:userid", "14201790")
                    .Build();

                options.AddPolicy("IsAdmin", isAlanPolicy);
            });

            services.AddDataProtection()
                    .SetApplicationName(nameof(AeBlog))
                    .PersistKeysToAwsS3(new S3XmlRepositoryConfig
                    {
                        Bucket = Environment.GetEnvironmentVariable("SESSION_BUCKET")
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
