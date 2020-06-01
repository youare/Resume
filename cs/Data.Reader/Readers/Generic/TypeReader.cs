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
    public interface ITypeReader<TKey, TType>
        where TType: BaseTypeDao<TKey>
    {
        Task<IList<TType>> GetAllAsync();
    }
    public class TypeReader<TKey, TType> : ITypeReader<TKey, TType>
              where TType : BaseTypeDao<TKey>
    {
        private readonly IDbConnection _connection;

        public TypeReader(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IList<TType>> GetAllAsync()
        {
            return (await _connection.QueryAsync<TType>($"select * from {typeof(TType).Name.FromPascalToSnakeCase()}")).ToList();
        }
    }
}
