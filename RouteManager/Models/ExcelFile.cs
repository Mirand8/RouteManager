using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace RouteManager.Models
{
    public class ExcelFile
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; private set; } = ObjectId.GenerateNewId().ToString();
        public List<BsonDocument> ExcelFiles { get; set; } = new List<BsonDocument>();
        public string FileName { get; set; }
    }
}
