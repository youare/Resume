using System;
using System.Threading.Tasks;
using Domain.Resume;
using Domain.SharedKernel.DomainEvents;

namespace BusinessLayer.Command.AggregateFactories
{
    public interface ITestAggregateFactory
    {
        Task<TestAggregate> GetAggregatAsync();
    }
    public class TestAggregateFactory: ITestAggregateFactory
    {
        private readonly IDomainEventDispatcher _domainEventDispatcher;

        public TestAggregateFactory(IDomainEventDispatcher domainEventDispatcher)
        {
            _domainEventDispatcher = domainEventDispatcher ?? throw new ArgumentNullException(nameof(domainEventDispatcher));
        }

        public async Task<TestAggregate> GetAggregatAsync()
        {
            return new TestAggregate(_domainEventDispatcher);
        }
    }
}
