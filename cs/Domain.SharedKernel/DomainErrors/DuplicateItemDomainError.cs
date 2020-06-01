using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.SharedKernel.DomainErrors
{
    public class DuplicateItemDomainError<T>:DomainError
    {
        public string Name { get; }
        public T Value { get; }

        public DuplicateItemDomainError(string name, T value)
        {
            Name = name;
            Value = value;
        }
    }
}
