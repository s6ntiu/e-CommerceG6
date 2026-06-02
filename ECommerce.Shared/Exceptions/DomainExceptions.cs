using System;

namespace ECommerce.Shared.Exceptions
{
    public class NotFoundException : Exception
    {
        public string ErrorCode { get; }
        public NotFoundException(string errorCode, string message) : base(message) { ErrorCode = errorCode; }
    }

    public class BusinessRuleException : Exception
    {
        public string ErrorCode { get; }
        public BusinessRuleException(string errorCode, string message) : base(message) { ErrorCode = errorCode; }
    }

    // NUEVAS EXCEPCIONES PARA EL TP
    public class UnauthorizedException : Exception
    {
        public string ErrorCode { get; }
        public UnauthorizedException(string errorCode, string message) : base(message) { ErrorCode = errorCode; }
    }

    public class ForbiddenException : Exception
    {
        public string ErrorCode { get; }
        public ForbiddenException(string errorCode, string message) : base(message) { ErrorCode = errorCode; }
    }

    public class UnprocessableEntityException : Exception
    {
        public string ErrorCode { get; }
        public UnprocessableEntityException(string errorCode, string message) : base(message) { ErrorCode = errorCode; }
    }
}
