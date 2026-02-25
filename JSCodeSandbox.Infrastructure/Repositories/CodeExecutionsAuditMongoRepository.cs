using AutoMapper;
using JSCodeSandbox.Application.Repositories;
using JSCodeSandbox.Exceptions;
using JSCodeSandbox.Infrastructure.Entities;
using JSCodeSandbox.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace JSCodeSandbox.Infrastructure.Repositories
{
    public class CodeExecutionsAuditMongoRepository : ICodeExecutionsAuditRepository
    {
        private readonly IMongoCollection<CodeExecutionAuditEntity> _collection;
        private readonly IMapper _mapper;
        private bool _collectionInitialized = false;
        private readonly SemaphoreSlim _initLock = new SemaphoreSlim(1, 1);
        private const string InvalidIdFormatMessage = "Invalid id format";

        public CodeExecutionsAuditMongoRepository(Configuration configuration, IMapper mapper)
        {
            _mapper = mapper;
            var client = new MongoClient(configuration.ConnectionString);
            var database = client.GetDatabase(configuration.DatabaseName);
            _collection = database.GetCollection<CodeExecutionAuditEntity>(configuration.CollectionName);
        }

        private async Task EnsureCollectionExistsAsync()
        {
            if (_collectionInitialized)
                return;

            await _initLock.WaitAsync();
            try
            {
                if (_collectionInitialized)
                    return;

                var database = _collection.Database;
                var collectionList = await database.ListCollectionNamesAsync();
                var collections = await collectionList.ToListAsync();

                if (!collections.Contains(_collection.CollectionNamespace.CollectionName))
                {
                    await database.CreateCollectionAsync(_collection.CollectionNamespace.CollectionName);
                }

                // Create indexes for better query performance
                var userAgentIndexKeys = Builders<CodeExecutionAuditEntity>.IndexKeys.Ascending(e => e.UserAgentId);
                var userAgentIndexModel = new CreateIndexModel<CodeExecutionAuditEntity>(userAgentIndexKeys);
                
                var environmentIndexKeys = Builders<CodeExecutionAuditEntity>.IndexKeys.Ascending(e => e.EnvironmentName);
                var environmentIndexModel = new CreateIndexModel<CodeExecutionAuditEntity>(environmentIndexKeys);
                
                var startedOnIndexKeys = Builders<CodeExecutionAuditEntity>.IndexKeys.Descending(e => e.StartedOnUTC);
                var startedOnIndexModel = new CreateIndexModel<CodeExecutionAuditEntity>(startedOnIndexKeys);

                await _collection.Indexes.CreateManyAsync(new[] { userAgentIndexModel, environmentIndexModel, startedOnIndexModel });

                _collectionInitialized = true;
            }
            finally
            {
                _initLock.Release();
            }
        }

        public async Task<CodeExecutionAudit> CreateAsync(CodeExecutionAudit codeExecution, CancellationToken cancellationToken = default)
        {
            await EnsureCollectionExistsAsync();
            var entity = _mapper.Map<CodeExecutionAuditEntity>(codeExecution);
            await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
            return _mapper.Map<CodeExecutionAudit>(entity);
        }

        public async Task<CodeExecutionAudit?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            if (!ObjectId.TryParse(id, out _))
                throw new ValidationException(InvalidIdFormatMessage);

            await EnsureCollectionExistsAsync();
            var filter = Builders<CodeExecutionAuditEntity>.Filter.Eq(e => e.Id, id);
            var entity = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return entity == null ? null : _mapper.Map<CodeExecutionAudit>(entity);
        }

        public async Task<(IEnumerable<CodeExecutionAudit> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            await EnsureCollectionExistsAsync();
            
            var totalCount = await _collection.CountDocumentsAsync(FilterDefinition<CodeExecutionAuditEntity>.Empty, cancellationToken: cancellationToken);
            
            var skip = (pageNumber - 1) * pageSize;
            var entities = await _collection.Find(FilterDefinition<CodeExecutionAuditEntity>.Empty)
                .SortByDescending(e => e.StartedOnUTC)
                .Skip(skip)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);

            var items = _mapper.Map<IEnumerable<CodeExecutionAudit>>(entities);
            return (items, (int)totalCount);
        }

        public async Task<(IEnumerable<CodeExecutionAudit> Items, int TotalCount)> GetByUserAgentIdAsync(string userAgentId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            await EnsureCollectionExistsAsync();
            
            var filter = Builders<CodeExecutionAuditEntity>.Filter.Eq(e => e.UserAgentId, userAgentId);
            var totalCount = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
            
            var skip = (pageNumber - 1) * pageSize;
            var entities = await _collection.Find(filter)
                .SortByDescending(e => e.StartedOnUTC)
                .Skip(skip)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);

            var items = _mapper.Map<IEnumerable<CodeExecutionAudit>>(entities);
            return (items, (int)totalCount);
        }

        public async Task<(IEnumerable<CodeExecutionAudit> Items, int TotalCount)> GetByEnvironmentNameAsync(string environmentName, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            await EnsureCollectionExistsAsync();
            
            var filter = Builders<CodeExecutionAuditEntity>.Filter.Eq(e => e.EnvironmentName, environmentName);
            var totalCount = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
            
            var skip = (pageNumber - 1) * pageSize;
            var entities = await _collection.Find(filter)
                .SortByDescending(e => e.StartedOnUTC)
                .Skip(skip)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);

            var items = _mapper.Map<IEnumerable<CodeExecutionAudit>>(entities);
            return (items, (int)totalCount);
        }

        public async Task<(IEnumerable<CodeExecutionAudit> Items, int TotalCount)> GetByErrorStatusAsync(bool isExecutionError, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            await EnsureCollectionExistsAsync();
            
            var filter = Builders<CodeExecutionAuditEntity>.Filter.Eq(e => e.IsExecutionError, isExecutionError);
            var totalCount = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
            
            var skip = (pageNumber - 1) * pageSize;
            var entities = await _collection.Find(filter)
                .SortByDescending(e => e.StartedOnUTC)
                .Skip(skip)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);

            var items = _mapper.Map<IEnumerable<CodeExecutionAudit>>(entities);
            return (items, (int)totalCount);
        }

        public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            if (!ObjectId.TryParse(id, out _))
                throw new ValidationException(InvalidIdFormatMessage);

            await EnsureCollectionExistsAsync();
            var filter = Builders<CodeExecutionAuditEntity>.Filter.Eq(e => e.Id, id);
            var result = await _collection.DeleteOneAsync(filter, cancellationToken);
            return result.DeletedCount > 0;
        }

        public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
        {
            if (!ObjectId.TryParse(id, out _))
                throw new ValidationException(InvalidIdFormatMessage);

            await EnsureCollectionExistsAsync();
            var filter = Builders<CodeExecutionAuditEntity>.Filter.Eq(e => e.Id, id);
            var count = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
            return count > 0;
        }

        public class Configuration
        {
            public string ConnectionString { get; set; } = string.Empty;
            public string DatabaseName { get; set; } = string.Empty;
            public string CollectionName { get; set; } = string.Empty;
        }
    }
}
