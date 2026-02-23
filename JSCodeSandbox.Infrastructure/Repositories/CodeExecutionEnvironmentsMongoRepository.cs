using AutoMapper;
using JSCodeSandbox.Application.Models;
using JSCodeSandbox.Application.Repositories;
using JSCodeSandbox.Infrastructure.Entities;
using MongoDB.Driver;


namespace JSCodeSandbox.Infrastructure.Repositories
{
    public class CodeExecutionEnvironmentsMongoRepository : ICodeExecutionEnvironmentsRepository
    {
        private readonly IMongoCollection<CodeExecutionEnvironmentEntity> _collection;
        private readonly IMapper _mapper;
        private bool _collectionInitialized = false;
        private readonly SemaphoreSlim _initLock = new SemaphoreSlim(1, 1);

        public CodeExecutionEnvironmentsMongoRepository(Configuration configuration, IMapper mapper)
        {
            _mapper = mapper;
            var client = new MongoClient(configuration.ConnectionString);
            var database = client.GetDatabase(configuration.DatabaseName);
            _collection = database.GetCollection<CodeExecutionEnvironmentEntity>(configuration.CollectionName);
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

                var indexKeys = Builders<CodeExecutionEnvironmentEntity>.IndexKeys.Ascending(e => e.EnvironmentName);
                var indexModel = new CreateIndexModel<CodeExecutionEnvironmentEntity>(indexKeys, new CreateIndexOptions { Unique = true });
                await _collection.Indexes.CreateOneAsync(indexModel);

                _collectionInitialized = true;
            }
            finally
            {
                _initLock.Release();
            }
        }

        public async Task CreateAsync(CodeExecutionEnvironment environment)
        {
            await EnsureCollectionExistsAsync();
            var entity = _mapper.Map<CodeExecutionEnvironmentEntity>(environment);
            await _collection.InsertOneAsync(entity);
        }

        public async Task<CodeExecutionEnvironment?> GetAsync(string environmentName)
        {
            await EnsureCollectionExistsAsync();
            var filter = Builders<CodeExecutionEnvironmentEntity>.Filter.Eq(e => e.EnvironmentName, environmentName);
            var entity = await _collection.Find(filter).FirstOrDefaultAsync();
            return entity == null ? null : _mapper.Map<CodeExecutionEnvironment>(entity);
        }

        public async Task DeleteAsync(string environmentName)
        {
            await EnsureCollectionExistsAsync();
            var filter = Builders<CodeExecutionEnvironmentEntity>.Filter.Eq(e => e.EnvironmentName, environmentName);
            await _collection.DeleteOneAsync(filter);
        }


        public class Configuration
        {
            public string ConnectionString { get; set; } = string.Empty;
            public string DatabaseName { get; set; } = string.Empty;
            public string CollectionName { get; set; } = string.Empty;
        }
    }
}
