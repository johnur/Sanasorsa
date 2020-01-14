using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RestApi.Model
{

    public class Wordlist
    {
        [JsonProperty("original")]
        public string Original { get; set; }

        [JsonProperty("guesses")]
        public string[] Guesses { get; set; }
    }
}
