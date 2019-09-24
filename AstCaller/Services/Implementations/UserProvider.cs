using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace AstCaller.Services.Implementations
{
    public class UserProvider : IUserProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? Id
        {
            get
            {
                if (int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var res))
                {
                    return res;
                }
                return null;
            }
        }

        public string Name => _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
    }
}
