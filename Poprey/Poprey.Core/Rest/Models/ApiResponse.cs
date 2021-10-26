using Newtonsoft.Json;

namespace Poprey.Core.Rest.Models
{
    public class ApiResponse<TResult>
    {
        [JsonProperty("result")]
        public string Result { get; set; }

        [JsonProperty("data")]
        public TResult Data { get; set; }

        [JsonProperty("csrf")]
        public string CsrfToken { get; set; }

        [JsonProperty("error_code")]
        public int? ErrorCode { get; set; }

        [JsonProperty("text")]
        public string ErrorText { get; set; }
    }
}