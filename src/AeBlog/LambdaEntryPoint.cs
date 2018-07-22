using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.AspNetCoreServer;
using Amazon.Lambda.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;

namespace AeBlog
{
    public class LambdaEntryPoint : APIGatewayProxyFunction
    {
        protected override void Init(IWebHostBuilder builder)
        {
            RegisterResponseContentEncodingForContentType("image/x-icon", ResponseContentEncoding.Base64);
            builder.UseStartup<Startup>();
        }

        protected override void PostMarshallRequestFeature(IHttpRequestFeature aspNetCoreRequestFeature, APIGatewayProxyRequest apiGatewayRequest, ILambdaContext lambdaContext)
        {
            if (apiGatewayRequest.Headers.ContainsKey("x-ae-domain") && apiGatewayRequest.Headers["x-ae-domain"] == "alanedwardes.com")
            {
                aspNetCoreRequestFeature.PathBase = string.Empty;
                aspNetCoreRequestFeature.Headers["Host"] = new StringValues("alanedwardes.com");
            }
        }
    }
}
