using System.Collections.Generic;
using System.Linq;

namespace Identity.Api.Application.Models.Errors
{
    public class ErrorResult
    {
        public IList<ErrorMessage> Errors { get; }
        public string[] ErrorCodes => Errors.Select(c => c.Code).ToArray();

        public ErrorResult()
        {
            Errors = new List<ErrorMessage>();
        }

        public ErrorResult(string code)
            : this()
        {
            Errors.Add(new ErrorMessage { Code = code });
        }

        public ErrorResult(string code, string message)
            : this()
        {
            Errors.Add(new ErrorMessage
            {
                Code = code,
                Message = message
            });
        }

        public ErrorResult(string code, string message, string requestId)
            : this()
        {
            Errors.Add(new ErrorMessage
            {
                Code = code,
                Message = message,
                RequestId = requestId
            });
        }

        public ErrorResult(ErrorMessage errorMessage)
            : this()
        {
            Errors.Add(errorMessage);
        }
    }
}