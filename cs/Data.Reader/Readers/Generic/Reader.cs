using Common.Extensions;
using Dapper;
using Data.Contracts.Daos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Reader.Readers.Generic
{
    public interface IReader<TKey, TBase>
         where TBase : BaseDao<TKey>
    {
        Task<IList<TBase>> GetAsync(DateTime date);
        Task<IList<TBase>> GetAllAsync();
        Task<IList<TBase>> GetAfterAsync(DateTime auditTime);
    }
    public class Reader<TKey, TBase> : IReader<TKey, TBase>
        where TBase:BaseDao<TKey>
    {
        private readonly IDbConnection _connection;

        public Reader(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IList<TBase>> GetAsync(DateTime date)
        {
            return (await _connection.QueryAsync<TBase>($"select * from {typeof(TBase).Name.FromPascalToSnakeCase()} where date=@date", new { date })).ToList();
        }
        public async Task<IList<TBase>> GetAfterAsync(DateTime auditTime)
        {
            return (await _connection.QueryAsync<TBase>($"select * from {typeof(TBase).Name.FromPascalToSnakeCase()} where audit_time >= @auditTime", new { auditTime })).ToList();
        }
        public async Task<IList<TBase>> GetAllAsync()
        {
            return (await _connection.QueryAsync<TBase>($"select * from {typeof(TBase).Name.FromPascalToSnakeCase()}")).ToList();
        }
    }
}
