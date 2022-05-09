using Newtonsoft.Json;

namespace Barriot.API.Voting
{
    public class VoteClient : IVoteClient
    {
        private readonly HttpClient _httpClient;
        private readonly DiscordRestClient _client;

        public VoteClient(HttpClient httpClient, DiscordRestClient client)
        {
            _httpClient = httpClient;
            _client = client;
        }

        /// <inheritdoc/>
        public async Task<bool> GetVoteAsync(RestUser user)
        {
            var response = await _httpClient.SendAsync(new HttpRequestMessage(
                method: HttpMethod.Get,
                requestUri: $"bots/{_client.CurrentUser.Id}/check?userId={user.Id}"));

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<Vote>(await response.Content.ReadAsStringAsync());
                return result?.HasVoted is 1;
            }
            return false;
        }
    }

    public class Vote
    {
        [JsonProperty("voted")]
        public int HasVoted { get; set; }
    }
}
