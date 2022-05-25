using Newtonsoft.Json;

namespace Barriot.API.Voting
{
    public class Vote
    {
        [JsonProperty("voted")]
        public int HasVoted { get; set; }
    }
}
