using System;
using Common;
using Domain.Resume.DomainEvents;
using Domain.SharedKernel.DomainErrors;
using Domain.SharedKernel.DomainEvents;
using Domain.SharedKernel.Entities;

namespace Domain.Resume
{
    public class TestAggregate : Aggregate
    {
        protected override IDomainEventDispatcher Dispatcher { get; }

        public TestAggregate(IDomainEventDispatcher dispatcher)
        {
            Dispatcher = dispatcher;
        }

        public Either<DomainError, int> ShouldPassOperation(int input)
        {
            Dispatcher.Enqueue(new TestPassEvent());
            return 10 + input;
        }

        public Either<DomainError, int> ShouldFailOperation(int input)
        {
            Dispatcher.Enqueue(new TestFailEvent());
            return new InvalidItemDomainError<int>(nameof(input), input);
        }
    }
}
