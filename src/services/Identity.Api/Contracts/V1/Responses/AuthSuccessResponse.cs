using System;

namespace Identity.Api.Contracts.V1.Responses
{
    public class AuthSuccessResponse
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
    }
}