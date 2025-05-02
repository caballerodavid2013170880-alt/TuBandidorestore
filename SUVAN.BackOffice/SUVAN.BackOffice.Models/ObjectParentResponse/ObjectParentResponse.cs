using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ObjectParentResponse
{
    public class ObjectParentResponse
    {
        [JsonPropertyName("Resultado")]
        public bool Resultado { get; set; } = false;

    }
}
