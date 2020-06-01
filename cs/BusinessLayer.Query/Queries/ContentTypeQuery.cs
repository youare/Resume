using Cache;
using Data.Contracts.Daos;
using Data.Reader.Readers.Generic;
using Domain.SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Query.Queries
{
    public interface IContentTypeQuery
    {
        Task<IList<ContentTypeValueObject>> GetAllValueObjectsAsync();
        Task<IDictionary<string, ContentTypeValueObject>> GetDictionaryAsync();
    }
    public class ContentTypeQuery: IContentTypeQuery
    {
        private readonly ITypeReader<string, ContentType> _contentTypeReader;
        private readonly ICache _cache;

        public ContentTypeQuery(ITypeReader<string, ContentType> contentTypeReader, ICache cache)
        {
            _contentTypeReader = contentTypeReader;
            _cache = cache;
        }

        public async Task<IList<ContentTypeValueObject>> GetAllValueObjectsAsync()
        {
            var cached = _cache.ContentTypes.Get();
            if (cached.HasValue) return cached.Value;
            var data = (await _contentTypeReader.GetAllAsync()).Select(x => x.ToValueObject()).ToList();
            _cache.ContentTypes.Set(data);
            return data;
        }
        public async Task<IDictionary<string, ContentTypeValueObject>> GetDictionaryAsync()
        {
            return (await GetAllValueObjectsAsync()).ToDictionary(x => x.Id);
        }
    }
}
