using System;
using System.Threading.Tasks;
using BusinessLayer.Command.Commands.Test;
using CorrelationId.Abstractions;
using Message;
using Microsoft.AspNetCore.Mvc;

namespace Resume.Api.Controllers
{
    public class TestController:BaseController
    {
        private readonly IDispatcher _dispatcher;
        private readonly ICorrelationContextAccessor _correlationContextAccessor;

        public TestController(IDispatcher dispatcher, ICorrelationContextAccessor correlationContextAccessor)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _correlationContextAccessor = correlationContextAccessor ?? throw new ArgumentNullException(nameof(correlationContextAccessor));
        }

        [HttpGet]
        [Route("pass")]
        public async Task PassAsync()
        {
            await _dispatcher.DispatchAsync(
                new TestPassCommand(
                    10,
                    "some type",
                    _correlationContextAccessor.CorrelationContext.CorrelationId,
                    DateTime.Now
                    )
                );
        }

        [HttpGet]
        [Route("fail")]
        public async Task FailAsync()
        {
            await _dispatcher.DispatchAsync(
                new TestFailCommand(
                    10,
                    "asdf",
                    _correlationContextAccessor.CorrelationContext.CorrelationId,
                    DateTime.Now)
                );
        }
    }
}
