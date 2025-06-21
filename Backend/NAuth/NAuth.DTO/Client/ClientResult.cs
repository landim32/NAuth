using NAuth.DTO.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NAuth.DTO.Client
{
    public class ClientResult: StatusResult
    {
        [JsonPropertyName("value")]
        public ClientInfo Value { get; set; }
    }
}
