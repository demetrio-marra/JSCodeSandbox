using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace JSCodeSandbox.Infrastructure.Entities
{
    public class CodeExecutionAuditEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        public DateTime StartedOnUTC { get; set; }

        public DateTime CompletedOnUTC { get; set; }

        public string CodeToRun { get; set; } = string.Empty;

        public string UserAgentId { get; set; } = string.Empty;

        public string EnvironmentName { get; set; } = string.Empty;

        public bool IsExecutionError { get; set; }

        public string ExecutionResult { get; set; } = string.Empty;

        public string Hostname { get; set; } = string.Empty;
    }
}
