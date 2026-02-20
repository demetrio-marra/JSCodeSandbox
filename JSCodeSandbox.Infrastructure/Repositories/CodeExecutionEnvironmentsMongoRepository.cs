using JSCodeSandbox.Application.Repositories;
using JSCodeSandbox.Models;
using MongoDB.Driver;


namespace JSCodeSandbox.Infrastructure.Repositories
{
    public class CodeExecutionEnvironmentsMongoRepository : ICodeExecutionEnvironmentsRepository
    {
        private readonly IMongoCollection<CodeExecutionEnvironment> _collection;
        private bool _collectionInitialized = false;
        private readonly SemaphoreSlim _initLock = new SemaphoreSlim(1, 1);

        public CodeExecutionEnvironmentsMongoRepository(Configuration configuration)
        {
            var client = new MongoClient(configuration.ConnectionString);
            var database = client.GetDatabase(configuration.DatabaseName);
            _collection = database.GetCollection<CodeExecutionEnvironment>(configuration.CollectionName);
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

                var indexKeys = Builders<CodeExecutionEnvironment>.IndexKeys.Ascending(e => e.EnvironmentName);
                var indexModel = new CreateIndexModel<CodeExecutionEnvironment>(indexKeys, new CreateIndexOptions { Unique = true });
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
            await _collection.InsertOneAsync(environment);
        }

        public async Task<CodeExecutionEnvironment?> GetAsync(string environmentName)
        {
            await EnsureCollectionExistsAsync();
            var filter = Builders<CodeExecutionEnvironment>.Filter.Eq(e => e.EnvironmentName, environmentName);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task DeleteAsync(string environmentName)
        {
            await EnsureCollectionExistsAsync();
            var filter = Builders<CodeExecutionEnvironment>.Filter.Eq(e => e.EnvironmentName, environmentName);
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
