using Newtonsoft.Json;
using System;

namespace ModelsLib
{
    public class Person : EntityBase
    {
		[MongoDB.Bson.Serialization.Attributes.BsonRequired]
		[JsonProperty("Name")]
		public string Name { get; set; }

		[MongoDB.Bson.Serialization.Attributes.BsonRequired]
		[JsonProperty("IsAvailableToTeam")]
		public bool IsAvailableToTeam { get; set; }
	}
}
