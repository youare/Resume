using System;
using System.Threading.Tasks;
using BusinessLayer.Command.AggregateFactories;
using BusinessLayer.Command.Commands.Test;

namespace BusinessLayer.Command.CommandHandlers.Test
{
    public class TestFailCommandHandler : ICommandHandler<int, TestFailCommand>
    {
        private readonly ITestAggregateFactory _testAggregateFactory;

        public TestFailCommandHandler(ITestAggregateFactory testAggregateFactory)
        {
            _testAggregateFactory = testAggregateFactory ?? throw new ArgumentNullException(nameof(testAggregateFactory));
        }

        public async Task HandleAsync(TestFailCommand command)
        {
            var aggregate = await _testAggregateFactory.GetAggregatAsync();
            aggregate.ShouldFailOperation(command.Data);
        }
    }
}
