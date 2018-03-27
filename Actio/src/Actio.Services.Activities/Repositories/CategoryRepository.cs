using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Actio.Services.Activities.Domain.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Actio.Services.Activities.Repositories
{
    public class CategoryRepository : Actio.Services.Activities.Domain.Repositories.ICategoryRepository
    {
        private IMongoDatabase _database;

        public CategoryRepository(IMongoDatabase database)
        {
            _database = database;
        }
        public async Task<IEnumerable<Category>> BrowseAsync() => await Collection.AsQueryable().ToListAsync();

        public async Task<Category> GetAsync(string name) => await Collection.AsQueryable().FirstOrDefaultAsync(x => x.Name == name.ToLowerInvariant());

        public async Task AddAsync(Category model) => await Collection.InsertOneAsync(model);

        private IMongoCollection<Category> Collection => _database.GetCollection<Category>("Categories");
    }
}