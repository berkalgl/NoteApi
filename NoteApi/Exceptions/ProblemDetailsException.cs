using Microsoft.AspNetCore.Mvc;

namespace NoteApi.Exceptions
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
