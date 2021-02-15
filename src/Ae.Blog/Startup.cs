using Ae.Freezer;
using Ae.Freezer.Aws;
using Ae.Freezer.Writers;
using Ae.Blog.Services;
using Amazon;
using Amazon.CloudFront;
using Amazon.DynamoDBv2;
using Amazon.S3;
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

namespace Ae.Blog
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
            services.AddSingleton<IAmazonS3>(new AmazonS3Client());

            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "config.json"), true)
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "config.secret.json"), true)
                .Build();

            services.AddHttpClient();

            services.AddFreezer()
                    .AddSingleton<IWebsiteResourceWriter>(x =>
                    {
                        var config = new AmazonS3WebsiteResourceWriterConfiguration
                        {
                            BucketName = configuration["S3_BUCKET"],
                            DistributionId = configuration["CLOUDFRONT_DISTRIBUTION"],
                            ShouldInvalidateCloudFrontCache = false,
                            ShouldCleanUnmatchedObjects = false
                        };

                        return new AmazonS3WebsiteResourceWriter(x.GetRequiredService<ILogger<AmazonS3WebsiteResourceWriter>>(), x.GetRequiredService<IAmazonS3>(), x.GetRequiredService<IAmazonCloudFront>(), config);
                    });

            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton(new HttpClient());

            services.AddSingleton(new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = configuration["YOUTUBE_API_KEY"],
                ApplicationName = "Ae.Blog"
            }));

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

            services.AddDataProtection()
                    .PersistKeysToAWSSystemsManager("/AeBlog/dataprotection");
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
