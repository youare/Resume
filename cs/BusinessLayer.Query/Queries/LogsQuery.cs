using BusinessLayer.Query.Dtos;
using BusinessLayer.Query.Dtos.Logs;
using Data.Contracts.Daos;
using Data.Reader.Readers.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Query.Queries
{
    public interface ILogsQuery
    {
        Task<IList<AnalysisLogReadDto>> GetAnalysisAsync(int pastNumOfDays);
        Task<IList<LogsReadDto>> GetAsync(DateTime date);
    }
    public class LogsQuery: ILogsQuery
    {
        private readonly IReader<Guid, Logs> _logsReader;

        public LogsQuery(IReader<Guid, Logs> logsReader)
        {
            _logsReader = logsReader;
        }
        public async Task<IList<AnalysisLogReadDto>> GetAnalysisAsync(int pastNumOfDays)
        {
            var time = DateTime.Now.AddDays(-pastNumOfDays);
            return (await _logsReader.GetAfterAsync(time))
                .GroupBy(x => x.SourceContext)
                .Select(x => new AnalysisLogReadDto(
                    x.Key,
                    x.Count(),
                    x.Sum(y => y.ElapsedMilliseconds.GetValueOrDefault() / x.Count()),
                    x.Select(y => y.ElapsedMilliseconds.GetValueOrDefault()).OrderBy(y => y).ToList()[x.Count() / 2]
                    )).ToList();
        }
        public async Task<IList<LogsReadDto>> GetAsync(DateTime date)
        {
            return (await _logsReader.GetAsync(date)).Select(LogsReadDto.MapFrom).ToList();
        }
    }
}
