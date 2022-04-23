using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsLib
{
    public class City : EntityBase
    {
        [BsonRequired]
        [JsonProperty("Name")]
        public string Name { get; set; }

        [BsonRequired]
        [JsonProperty("State")]
        public string State { get; set; }
    }
}
