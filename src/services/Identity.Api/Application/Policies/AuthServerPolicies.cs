using System;
using System.Collections.Generic;
using Identity.Api.Data;
using Identity.Api.Infrastructure.Extensions.Auth;
using Microsoft.AspNetCore.Authorization;

namespace Identity.Api.Application.Policies
{
    public class AuthServerPolicies : ICustomPolicyBuilder
    {
        public const string RolePermissionsPolicy = "RolePermissionsPolicy";
        public const string RolePermissionsPolicyTest = "RolePermissionsPolicyTest";

        private const string RequireRoleAdministratorPolicy = "RequireAdministratorRolePolicy";
        private const string RequireRoleCompanyPolicy = "RequireRolecCompanyPolicy";
        private const string RequireRoleSupportPolicy = "RequireSupportRolePolicy";

        public Dictionary<string, Action<AuthorizationPolicyBuilder>> GetPolicies() =>
            new()
            {
                { RolePermissionsPolicy, policy => policy.RequireClaim("RolePermissionsChange", "allow") },
                { RolePermissionsPolicyTest, policy => policy.RequireClaim("RolePermissionsChangeTest", "allow") },
                { RequireRoleAdministratorPolicy, policy => policy.RequireRole(Roles.Admin) },
                { RequireRoleCompanyPolicy, policy => policy.RequireRole(Roles.Company) },
                { RequireRoleSupportPolicy, policy => policy.RequireRole(Roles.Support) }
            };
    }
}