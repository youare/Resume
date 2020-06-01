using System;
using System.Collections.Generic;
using System.Text;
using Domain.SharedKernel.DomainEvents;

namespace Domain.SharedKernel.Entities
{
    public abstract class Aggregate
    {
        protected abstract IDomainEventDispatcher Dispatcher {get;}
    }
}
