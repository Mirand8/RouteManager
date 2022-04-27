using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.Collections.Generic;

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
        [JsonProperty("City")]
        public City City { get; set; }
    }
}
