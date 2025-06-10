using Microsoft.AspNetCore.Mvc;
using Proto.IdentityV1;

namespace Gateway.Authorization;

public class AuthorizationAttribute : TypeFilterAttribute
{
    public AuthorizationAttribute(params Role[] roles) : base(typeof(AuthorizationFilter))
    {
        Arguments = [roles];
    }
}