using System;
using System.Text.Json.Serialization;
using NAuth.DTO.Domain;

namespace NAuth.DTO.Configuration
{
    public class VersionResult : StatusResult
    {
        [JsonPropertyName("version")]
        public string Version { get; set; }
    }
}
