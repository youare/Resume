using Domain.SharedKernel.DomainErrors;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Command.DomainErrorHandlers
{
    public interface IDomainErrorHandler
    {
        IDomainErrorHandler SetNext(IDomainErrorHandler handler);
        IList<string> Handle(DomainError error, IList<string> errors = null);
    }
    public class DomainErrorHandler<TDomainError>: IDomainErrorHandler
        where TDomainError : DomainError
    {
        private readonly bool _shortCircuit;
        private readonly string _customErrorMessage;
        private IDomainErrorHandler _next;
        private const string DefaultErrorMessage = "Error occured, please contact us.";
        public DomainErrorHandler(string customErrorMessage = "", bool shortCircuit = true)
        {
            _shortCircuit = shortCircuit;
            _customErrorMessage = customErrorMessage;
        }
        public IDomainErrorHandler SetNext(IDomainErrorHandler handler)
        {
            if (_next is null) _next = handler;
            else _next.SetNext(handler);
            return this;
        }
        public IList<string> Handle(DomainError error, IList<string> errors = null)
        {
            if (errors is null) errors = new List<string>();
            if(error is TDomainError)
            {
                if (!string.IsNullOrEmpty(_customErrorMessage)) errors.Add(_customErrorMessage);
                else errors.Add(getErrorMessage(error));
                if (_shortCircuit) return errors;
            }
            return _next?.Handle(error, errors) ?? new List<string> { DefaultErrorMessage };
        }
        private string getErrorMessage(DomainError error)
        {
            switch (error)
            {
                case InvalidItemDomainError<int> err: return $"{err.Name}:{err.Value} is invalid.";
                default:
                    return DefaultErrorMessage;
            }
        }
    }
}
