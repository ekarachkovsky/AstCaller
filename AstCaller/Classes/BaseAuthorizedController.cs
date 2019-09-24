using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AstCaller.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace AstCaller.Controllers
{
    [Authorize]
    public class BaseAuthorizedController : BaseController
    {
        protected readonly int? _currentUserId;
        protected readonly string _currentUserName;

        public BaseAuthorizedController(ILogger<BaseAuthorizedController> logger, IUserProvider userProvider) : base(logger)
        {
            _currentUserId = userProvider.Id;
            _currentUserName = userProvider.Name;
        }
    }
}
