using Common;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace Cache
{
    public interface ICacheEntry<TEntity>
    {
        Optional<TEntity> Get();
        void Set(TEntity value);
        void Clear();
    }
    public class CacheEntry<TEntity>: ICacheEntry<TEntity>
    {
        private readonly IMemoryCache _memoryCache;
        private readonly string _name;
        private readonly MemoryCacheEntryOptions _memoryCacheEntryOptions;

        public CacheEntry(IMemoryCache memoryCache, string name, MemoryCacheEntryOptions memoryCacheEntryOptions)
        {
            _memoryCache = memoryCache;
            _name = name;
            _memoryCacheEntryOptions = memoryCacheEntryOptions;
        }

        public Optional<TEntity> Get()
        {
            if(_memoryCache.TryGetValue<TEntity>(_name, out var data))
            {
                return Optional<TEntity>.Some(data);
            }
            return Optional<TEntity>.Empty;
        }
        public void Set(TEntity value)
        {
            _memoryCache.Set(_name, value, _memoryCacheEntryOptions);
        }
        public void Clear()
        {
            _memoryCache.Remove(_name);
        }
    }
}
