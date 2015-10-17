using AeBlog.Options;
using Microsoft.Framework.OptionsModel;

namespace AeBlog.Clients
{
    public class LastfmClientFactory : ILastfmClientFactory
    {
        private readonly IOptions<Credentials> credentials;

        public LastfmClientFactory(IOptions<Credentials> credentials)
        {
            this.credentials = credentials;
        }

        public ILastfmClient CreateLastfmClient()
        {
            return new LastfmClient(credentials.Value.LastFmApiKey, credentials.Value.LastFmUsername);
        }
    }
}
