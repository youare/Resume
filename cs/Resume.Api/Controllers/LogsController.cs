using BusinessLayer.Query.Dtos;
using BusinessLayer.Query.Dtos.Logs;
using BusinessLayer.Query.Queries;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Resume.Api.Controllers
{
    public class LogsController:BaseController
    {
        private readonly ILogsQuery _logsQuery;

        public LogsController(ILogsQuery logsQuery)
        {
            _logsQuery = logsQuery;
        }
        [HttpGet]
        [Route("")]
        public async Task<IList<LogsReadDto>> GetAsync(DateTime date)
        {
            return await _logsQuery.GetAsync(date);
        }
        [HttpGet]
        [Route("analysis")]
        public async Task<IList<AnalysisLogReadDto>> GetAnalysisLogAsync()
        {
            return await _logsQuery.GetAnalysisAsync(10);
        }
    }
}
