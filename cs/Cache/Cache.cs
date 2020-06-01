using Domain.SharedKernel.ValueObjects;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cache
{
    public interface ICache
    {
        ICacheEntry<IList<ContentTypeValueObject>> ContentTypes { get; }
        ICacheEntry<IList<SectionTypeValueObject>> SectionTypes { get; }
    }
    public class Cache : ICache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions DefaultCacheEntryOptions = new MemoryCacheEntryOptions { Priority = CacheItemPriority.NeverRemove };

        public Cache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            ContentTypes = new CacheEntry<IList<ContentTypeValueObject>>(_memoryCache, nameof(ContentTypes), DefaultCacheEntryOptions);
            SectionTypes = new CacheEntry<IList<SectionTypeValueObject>>(_memoryCache, nameof(SectionTypes), DefaultCacheEntryOptions);
        }
        public ICacheEntry<IList<ContentTypeValueObject>> ContentTypes {get;}
        public ICacheEntry<IList<SectionTypeValueObject>> SectionTypes { get; }

    }
}
