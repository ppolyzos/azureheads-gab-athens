using System;
using System.Linq;
using Identity.Api.Application.Models.Errors;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Application.Models
{
    public class ResponseResult : ErrorResult
    {
        public object Result { get; }
        public bool Success => Errors == null || !Errors.Any();

        public ResponseResult(object result)
        {
            Result = result;
        }

        public ResponseResult(object result, string code)
            : base(code)
        {
            Result = result;
        }

        public ResponseResult(object result, string code, string message)
            : base(code, message)
        {
            Result = result;
        }

        public ResponseResult(object result, Enum code) : this(result, code.ToString()) { }

        public ResponseResult(object result, ErrorMessage errorMessage)
            : this(result)
        {
            AddError(errorMessage);
        }

        public ResponseResult(object result, IdentityResult identityResult)
            : this(result)
        {
            foreach (var error in identityResult.Errors)
            {
                AddError(new ErrorMessage
                {
                    Code = error.Code,
                    Message = error.Description
                });
            }
        }

        public void AddError(ErrorMessage errorMessage)
        {
            Errors.Add(errorMessage);
        }
    }
}
