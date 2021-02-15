using Amazon.Lambda.AspNetCoreServer;
using Microsoft.AspNetCore.Hosting;

namespace Ae.Blog
{
    public class LambdaEntryPoint : APIGatewayProxyFunction
    {
        protected override void Init(IWebHostBuilder builder)
        {
            RegisterResponseContentEncodingForContentType("image/x-icon", ResponseContentEncoding.Base64);
            builder.UseStartup<Startup>();
        }
    }
}
