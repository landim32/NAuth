﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NAuth.DTO.Domain
{
    public class NumberResult: StatusResult
    {
        [JsonPropertyName("value")]
        public double Value { get; set; }
    }
}
