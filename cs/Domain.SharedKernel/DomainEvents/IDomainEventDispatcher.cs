using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SharedKernel.DomainEvents
{
    public interface IDomainEventDispatcher
    {
        void Enqueue(IDomainEvent domainEvent);
        Task DispatchAsync(IServiceProvider serviceProvider);
    }
}
