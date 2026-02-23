using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace JSCodeSandbox.Infrastructure.Entities
{
    public class CodeExecutionEnvironmentEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        public string EnvironmentName { get; set; } = string.Empty;
        
        public Dictionary<string, string> BackendUrls { get; set; } = new Dictionary<string, string>();
        
        public string CodeImplementation { get; set; } = string.Empty;
    }
}
