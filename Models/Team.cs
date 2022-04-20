using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsLib
{
    public class Team : EntityBase
    {
        [BsonRequired]
        [JsonProperty("Name")]
        public string Name { get; set; }
        [BsonRequired]
        [JsonProperty("Members")]
        public List<Person> Members { get; set; }
        [BsonRequired]
        [JsonProperty("IsAvailable")]
        public bool IsAvailable { get; set; }
        [BsonRequired]
        [JsonProperty("State")]
        public string State { get; set; }
        [BsonRequired]
        [JsonProperty("City")]
        public string City { get; set; }
    }
}
