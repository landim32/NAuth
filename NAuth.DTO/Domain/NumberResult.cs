using System.Text.Json.Serialization;

namespace NAuth.DTO.Domain
{
    public class NumberResult : StatusResult
    {
        [JsonPropertyName("value")]
        public double Value { get; set; }
    }
}
