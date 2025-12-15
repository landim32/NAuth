using System;
using System.Text.Json.Serialization;
using NAuth.DTO.Domain;

namespace NAuth.DTO.User
{
    public class BalanceResult : StatusResult
    {
        [JsonPropertyName("balance")]
        public BalanceInfo Balance { get; set; }
    }
}
