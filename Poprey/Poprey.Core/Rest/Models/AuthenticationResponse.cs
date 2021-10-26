using Newtonsoft.Json;

namespace Poprey.Core.Rest.Models
{
    public class AuthenticationResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("balance")]
        public string Balance { get; set; }

        [JsonProperty("discount")]
        public string Discount { get; set; }
    }
}
