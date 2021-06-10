using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Identity.Api.Infrastructure.Extensions.Auth
{
    public interface ICustomPolicyBuilder
    {
        Dictionary<string, Action<AuthorizationPolicyBuilder>> GetPolicies();
    }
}