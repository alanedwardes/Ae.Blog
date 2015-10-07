using AeBlog.Options;
using Microsoft.Framework.OptionsModel;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace AeBlog.Tasks
{
    public class MapMyRunTask : IScheduledTask
    {
        private readonly IOptions<Credentials> credentials;

        public MapMyRunTask(IOptions<Credentials> credentials)
        {
            this.credentials = credentials;
        }

        public async Task<TimeSpan> DoWork(CancellationToken ctx)
        {
            using (var client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" },
                    { "client_id", credentials.Options.UnderArmourClientId },
                    { "client_secret", credentials.Options.UnderArmourClientSecret }
                });
                content.Headers.Add("Api-Key", credentials.Options.UnderArmourClientId);

                var response = await client.SendAsync(new HttpRequestMessage
                {
                    RequestUri = new Uri("https://api.ua.com/v7.1/oauth2/access_token/"),
                    Method = HttpMethod.Post,
                    Content = content
                });

                var token = JToken.Parse(await response.Content.ReadAsStringAsync())["access_token"].Value<string>();

                var request = new HttpRequestMessage(HttpMethod.Get, new Uri("https://api.ua.com/v7.1/user/" + credentials.Options.UnderArmourUserId));
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                request.Headers.Add("Api-Key", credentials.Options.UnderArmourClientId);
                var response2 = await client.SendAsync(request);

                var json = await response2.Content.ReadAsStringAsync();
                var userdata = JToken.Parse(json);

                var lifetime = userdata["_links"]["stats"].Where(j => j["name"].ToObject<string>() == "lifetime").Single()["href"].ToObject<string>();

                {

                    var req = new HttpRequestMessage(HttpMethod.Get, new Uri("https://api.ua.com" + lifetime));
                    req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    req.Headers.Add("Api-Key", credentials.Options.UnderArmourClientId);
                    var res = await client.SendAsync(req);

                    var c = await res.Content.ReadAsStringAsync();
                }
            }

            return Timeout.InfiniteTimeSpan;
        }
    }
}
