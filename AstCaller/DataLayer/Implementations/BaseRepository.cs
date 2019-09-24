using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AstCaller.Models.Domain;
using Dapper;

namespace AstCaller.DataLayer.Implementations
{
    public abstract class BaseRepository<TModel> where TModel : BaseModel
    {
        protected readonly ISqlConnectionProvider _connectionProvider;
        private readonly string _tableName;
        private static readonly ConcurrentDictionary<string, string[]> _tableFields = new ConcurrentDictionary<string, string[]>();

        public BaseRepository(ISqlConnectionProvider connectionProvider, string tableName)
        {
            _connectionProvider = connectionProvider;
            _tableName = tableName;
        }

        public virtual async Task<TModel> GetAsync(int id)
        {
            using (var db = _connectionProvider.GetConnection())
            {
                return await db.QuerySingleOrDefaultAsync<TModel>($"select * from {_tableName} where id=@id", new { id });
            }
        }

        public virtual async Task<int> TotalCount()
        {
            using (var db = _connectionProvider.GetConnection())
            {
                return await db.QuerySingleOrDefaultAsync<int>($"select count(*) from {_tableName}");
            }
        }

        public virtual async Task<IEnumerable<TResult>> GetRange<TResult>(int rangeBegin, int pageSize, string viewName)
        {
            using (var db = _connectionProvider.GetConnection())
            {
                return await db.QueryAsync<TResult>($"select * from {viewName} limit @rangeBegin, @pageSize", new { rangeBegin, pageSize });
            }
        }

        public virtual async Task<int> SaveAsync(TModel model)
        {
            using (var db = _connectionProvider.GetConnection())
            {
                var props = GetProps();
                if (model.Id > 0)
                {
                    await db.ExecuteAsync($"update {_tableName} set {string.Join(",", props.Select(x => $"{x}=@{x}"))} where id=@id", model);
                }
                else
                {
                    await db.ExecuteAsync($"insert into {_tableName} ({string.Join(",", props)}) values({string.Join(",", props.Select(x => $"@{x}"))})", model);
                    model.Id = await db.QueryFirstOrDefaultAsync<int>("SELECT LAST_INSERT_ID();");
                }
            }

            return model.Id;
        }

        public virtual async Task DeleteAsync(int id)
        {
            using (var db = _connectionProvider.GetConnection())
            {
                await db.ExecuteAsync($"delete from {_tableName} where id=@id", new { id });
            }
        }

        private string[] GetProps()
        {
            if (!_tableFields.TryGetValue(_tableName, out var result))
            {
                result = typeof(TModel).GetProperties().Select(x => x.Name).ToArray();
                _tableFields.TryAdd(_tableName, result);
            }

            return result;
        }
    }
}
