using Ae.Freezer;
using Ae.Freezer.Aws;
using Ae.Freezer.Writers;
using Ae.Blog.Services;
using Amazon;
using Amazon.CloudFront;
using Amazon.DynamoDBv2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net.Http;
using Amazon.Lambda;
using System;
using Amazon.S3;

namespace Ae.Blog
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSingleton<IColourRepository, ColourRepository>();
            services.AddSingleton<IBlogPostRepository, BlogPostRepository>();
            services.AddSingleton<IAmazonS3>(new AmazonS3Client(RegionEndpoint.EUWest2));
            services.AddSingleton<IAmazonDynamoDB>(new AmazonDynamoDBClient(RegionEndpoint.EUWest1));
            services.AddSingleton<IAmazonCloudFront>(new AmazonCloudFrontClient());
            services.AddSingleton<IAmazonLambda>(new AmazonLambdaClient(RegionEndpoint.USEast1));
            services.AddSingleton<IAmazonIdentityManagementService>(new AmazonIdentityManagementServiceClient());

            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "config.json"), true)
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "config.secret.json"), true)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            services.AddHttpClient();

            services.AddHttpClient("FREEZER_CLIENT", x =>
            {
                x.BaseAddress = new Uri(configuration["BASE_ADDRESS"]);
                x.DefaultRequestHeaders.Add("Freezing", "1");
            });

            services.AddFreezer()
                    .AddSingleton<IWebsiteResourceWriter>(x =>
                    {
                        return new AmazonS3WebsiteResourceWriter(x.GetRequiredService<ILogger<AmazonS3WebsiteResourceWriter>>(), new AmazonS3WebsiteResourceWriterConfiguration
                        {
                            BucketName = configuration["PUBLISH_BUCKET"],
                            DistributionId = configuration["CLOUDFRONT_DISTRIBUTION"],
                            PutRequestModifier = request => request.CannedACL = S3CannedACL.PublicRead,
                            ShouldCleanUnmatchedObjects = true,
                        }, x.GetRequiredService<IAmazonS3>(), x.GetRequiredService<IAmazonCloudFront>());
                    });

            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton(new HttpClient());

            services.AddSingleton(new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = configuration["YOUTUBE_API_KEY"],
                ApplicationName = "Ae.Blog"
            }));

            services.AddRouting(options => options.AppendTrailingSlash = true);

            services.AddSingleton<PageRouteTransformer>();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseDeveloperExceptionPage();

            loggerFactory.AddLambdaLogger();

            app.UseStaticFiles();
            app.UseStatusCodePagesWithReExecute("/error");

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDynamicControllerRoute<PageRouteTransformer>("/");
                endpoints.MapDynamicControllerRoute<PageRouteTransformer>("/{page}/");
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
