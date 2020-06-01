using System;
using System.Threading.Tasks;
using BusinessLayer.Command.AggregateFactories;
using BusinessLayer.Command.Commands.Test;

namespace BusinessLayer.Command.CommandHandlers.Test
{
    public class TestPassCommandHandler:ICommandHandler<int, TestPassCommand>
    {
        private readonly ITestAggregateFactory _testAggregateFactory;

        public TestPassCommandHandler(ITestAggregateFactory testAggregateFactory)
        {
            _testAggregateFactory = testAggregateFactory ?? throw new ArgumentNullException(nameof(testAggregateFactory));
        }

        public async Task HandleAsync(TestPassCommand command)
        {
            var aggregate = await _testAggregateFactory.GetAggregatAsync();
            aggregate.ShouldPassOperation(command.Data);
        }
    }
}
