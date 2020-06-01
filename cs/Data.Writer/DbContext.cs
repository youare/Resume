using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Writer
{
    public interface IDbContext
    {
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        void Dispose();
        ValueTask DisposeAsync();
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

    }
    public class DbContext:Microsoft.EntityFrameworkCore.DbContext, IDbContext
    {
        private readonly string _connectionString;

        public DbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLazyLoadingProxies()
                .UseNpgsql(_connectionString)
                .UseSnakeCaseNamingConvention();
        }
    }
}
