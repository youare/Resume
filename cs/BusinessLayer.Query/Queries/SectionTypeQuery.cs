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
    public interface ISectionTypeQuery
    {
        Task<IList<SectionTypeValueObject>> GetAllValueObjectsAsync();
        Task<IDictionary<string, SectionTypeValueObject>> GetDictionaryAsync();
    }
    public class SectionTypeQuery : ISectionTypeQuery
    {
        private readonly ITypeReader<string, SectionType> _sectionTypeReader;
        private readonly ICache _cache;

        public SectionTypeQuery(ITypeReader<string, SectionType> sectionTypeReader, ICache cache)
        {
            _sectionTypeReader = sectionTypeReader;
            _cache = cache;
        }

        public async Task<IList<SectionTypeValueObject>> GetAllValueObjectsAsync()
        {
            var cached = _cache.SectionTypes.Get();
            if (cached.HasValue) return cached.Value;
            var data = (await _sectionTypeReader.GetAllAsync()).Select(x => x.ToValueObject()).ToList();
            _cache.SectionTypes.Set(data);
            return data;
        }
        public async Task<IDictionary<string, SectionTypeValueObject>> GetDictionaryAsync()
        {
            return (await GetAllValueObjectsAsync()).ToDictionary(x => x.Id);
        }
    }
}
