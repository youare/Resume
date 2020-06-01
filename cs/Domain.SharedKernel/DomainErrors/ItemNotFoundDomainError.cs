using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.SharedKernel.DomainErrors
{
    public class ItemNotFoundDomainError<T>:DomainError
    {
        public string Name { get; }
        public T Value { get; }

        public ItemNotFoundDomainError(string name, T value)
        {
            Name = name;
            Value = value;
        }
    }
}
