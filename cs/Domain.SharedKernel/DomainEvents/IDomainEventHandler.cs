using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SharedKernel.DomainEvents
{
    public interface IDomainEventHandler<TDomainEvent> where TDomainEvent: IDomainEvent
    {
        Task HandleAsync(TDomainEvent domainEvent, IServiceProvider serviceProvider);
    }
}
