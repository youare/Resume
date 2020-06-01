using System;
using System.Threading.Tasks;
using Data.Contracts.Daos;
using Microsoft.EntityFrameworkCore;

namespace Data.Writer.Writers.Generic
{
    public interface ITpyeWriter<TKey, TType>
        where TType: BaseTypeDao<TKey>
    {
        Task AddAsync(TType item);
        Task UpdateAsync(TType item);
        Task DeleteAsync(TKey id);
    }
    public class TypeWriter<TKey, TType>: ITpyeWriter<TKey, TType>
        where TType : BaseTypeDao<TKey>
    {
        private readonly IDbContext _context;

        public TypeWriter(IDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task AddAsync(TType item)
        {
            await _context.Set<TType>().AddAsync(item);
        }
        public async Task UpdateAsync(TType item)
        {
            _context.Set<TType>().Update(item);
        }
        public async Task DeleteAsync(TKey id)
        {
            var toBeRemoved = await _context.Set<TType>().FirstOrDefaultAsync(x => Equals(x.Id, id));
            if (!(toBeRemoved is null)) _context.Set<TType>().Remove(toBeRemoved);
        }
    }
}
