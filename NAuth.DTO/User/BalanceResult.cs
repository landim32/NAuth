using NAuth.DTO.Domain;
using System.Text.Json.Serialization;

namespace NAuth.DTO.User
{
    public class BalanceResult : StatusResult
    {
        [JsonPropertyName("balance")]
        public BalanceInfo Balance { get; set; }
    }
}
