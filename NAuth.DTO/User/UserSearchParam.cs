using System.Text.Json.Serialization;

namespace NAuth.DTO.User
{
    public class UserSearchParam
    {
        [JsonPropertyName("networkId")]
        public long NetworkId { get; set; }
        [JsonPropertyName("keyword")]
        public string Keyword { get; set; }
        [JsonPropertyName("profileId")]
        public long? ProfileId { get; set; }
        [JsonPropertyName("pageNum")]
        public int PageNum { get; set; }
    }
}
