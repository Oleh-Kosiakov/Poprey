using System.Collections.Generic;
using Newtonsoft.Json;

namespace Poprey.Core.Models.Instagram
{
    public class InstagramAccount
    {
        public string InstagramNickname { get; set; }

        [JsonProperty("avatar")]
        public string InstagramAvatarUri { get; set; }

        [JsonProperty("posts")]
        public IEnumerable<InstagramPost> Posts { get; set; }
    }
}