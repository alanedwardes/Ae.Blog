using AeBlog.Services;
using Amazon;
using Amazon.CloudFront;
using Amazon.DynamoDBv2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net.Http;

namespace AeBlog
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSingleton<IImageRepository, ImageRepository>();
            services.AddSingleton<IColourRepository, ColourRepository>();
            services.AddSingleton<IBlogPostRepository, BlogPostRepository>();
            services.AddSingleton<IAmazonDynamoDB>(new AmazonDynamoDBClient(RegionEndpoint.EUWest1));
            services.AddSingleton<IAmazonCloudFront>(new AmazonCloudFrontClient());

            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "config.json"), true)
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "config.secret.json"), true)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton(new HttpClient());

            services.AddSingleton(new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = configuration["YOUTUBE_API_KEY"],
                ApplicationName = "aeblog"
            }));

            services.AddSingleton<ICloudFrontInvalidator>(x => new CloudFrontInvalidator(configuration["CLOUDFRONT_DISTRIBUTION"], x.GetRequiredService<IAmazonCloudFront>()));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {
                        options.LoginPath = "/Admin/login";
                        options.AccessDeniedPath = "/Admin/denied";
                    })
                    .AddTwitter(options =>
                    {
                        options.CallbackPath = "/Admin/auth/twitter-signin";
                        options.ConsumerKey = configuration["TWITTER_CONSUMER_KEY"];
                        options.ConsumerSecret = configuration["TWITTER_CONSUMER_SECRET"];
                    });

            services.AddAuthorization(options => options.AddPolicy("IsAdmin", x => {
                x.RequireClaim("urn:twitter:userid", "14201790").AddAuthenticationSchemes(TwitterDefaults.AuthenticationScheme);
            }));

            services.AddRouting(options => options.AppendTrailingSlash = true);

            services.AddMemoryCache();

            services.AddDataProtection()
                    .PersistKeysToAWSSystemsManager("/aeblog/dataprotection");
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseExceptionHandler("/error");

            loggerFactory.AddLambdaLogger();

            app.UseStaticFiles();
            app.UseStatusCodePagesWithReExecute("/error");

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
