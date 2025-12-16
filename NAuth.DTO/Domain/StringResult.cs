using System.Text.Json.Serialization;

namespace NAuth.DTO.Domain
{
    public class StringResult : StatusResult
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
