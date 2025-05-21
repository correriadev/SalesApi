using System;

namespace SalesApi.Domain.Exceptions;

public class DomainException : Exception
{
    public string ErrorType { get; }
    public string Error { get; }

    public DomainException(string errorType, string error, string message) 
        : base(message)
    {
        ErrorType = errorType;
        Error = error;
    }
} 