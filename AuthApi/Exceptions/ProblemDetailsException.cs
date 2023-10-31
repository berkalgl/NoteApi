using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Exceptions
{
    public class ProblemDetailsException : Exception
    {
        public ProblemDetailsException(ProblemDetails problemDetails)
        {
            Value = problemDetails;
        }

        public ProblemDetails Value { get; }
    }
}
