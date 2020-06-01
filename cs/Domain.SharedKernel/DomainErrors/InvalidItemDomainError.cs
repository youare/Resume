using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.SharedKernel.DomainErrors
{
    public class InvalidItemDomainError<T> : DomainError 
    {
        public string Name { get; }
        public T Value { get; }

        public InvalidItemDomainError(string name, T value)
        {
            Name = name;
            Value = value;
        }
    }
}
