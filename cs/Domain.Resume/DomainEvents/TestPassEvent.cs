using System;
using Domain.SharedKernel.DomainEvents;

namespace Domain.Resume.DomainEvents
{
    public class TestPassEvent: IDomainEvent
    {
        public TestPassEvent()
        {
        }
    }
    public class TestFailEvent: IDomainEvent
    {

    }
}
