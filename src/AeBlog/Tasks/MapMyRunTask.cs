using AeBlog.Options;
using Microsoft.Framework.OptionsModel;
using Newtonsoft.Json.Linq;
using System;
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

                var userdata = await response2.Content.ReadAsStringAsync();
            }

            return Timeout.InfiniteTimeSpan;
        }
    }
}
