using Newtonsoft.Json;
using System;

namespace ModelsLib
{
    public class Person : EntityBase
    {
		[MongoDB.Bson.Serialization.Attributes.BsonRequired]
		[JsonProperty("Name")]
		public string Name { get; set; }

		[JsonProperty("CurrentTeam")]
		public string CurrentTeam { get; set; }

	}
}
