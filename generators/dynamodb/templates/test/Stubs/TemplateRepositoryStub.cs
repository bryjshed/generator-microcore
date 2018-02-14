using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.Logging;


namespace <%= namespace %>.Tests
{
    public class <%= modelName %>RepositoryStub : I<%= modelName %>Repository
    {
        private IDictionary<string, <%= modelName %>> _localStore;

        public <%= modelName %>RepositoryStub(ILogger<<%= modelName %>RepositoryStub> logger, IDynamoDBContext context)
        {
            _localStore = new Dictionary<string, <%= modelName %>>();
        }

        public Task SaveAsync(<%= modelName %> model)
        {
            model.Id = Guid.NewGuid().ToString();
            _localStore.Add(model.Id, model);

            return Task.FromResult<<%= modelName %>>(model);
        }

        public Task UpdateAsync(<%= modelName %> model)
        {
            if (!_localStore.ContainsKey(model.Id))
            {
                Task.FromResult<<%= modelName %>>(null);
            }
            var fetch = _localStore[model.Id];
            if (fetch != null)
            {
                _localStore[model.Id] = model;
            }
            return Task.FromResult<<%= modelName %>>(model);
        }

        public Task<<%= modelName %>> GetByIdAsync(string id)
        {
            if (!_localStore.ContainsKey(id))
            {
                return Task.FromResult<<%= modelName %>>(null);
            }
            return Task.FromResult<<%= modelName %>>(_localStore[id]);
        }

        public async Task<IQueryable<<%= modelName %>>> FindAsync(IEnumerable<ScanCondition> conditions, string lastId, int? pageSize)
        {
            var result = _localStore.Values.AsQueryable();

            if (lastId != null && lastId != "")
            {
                result = result.SkipWhile(r => (r.Id != lastId)).Skip(1);
            }

            if (pageSize.HasValue)
            {
                result = result.Take(pageSize.Value);
            }

            return await Task.FromResult<IQueryable<<%= modelName %>>>(result);
        }

        public void ClearDb()
        {
            _localStore = new Dictionary<string, <%= modelName %>>();
        }
    }
}
