using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AstCaller.Areas.Administration.Controllers
{
    public class OrganizationController : Controller
    {
        public IActionResult Index(int page = 0)
        {
            ViewBag.Page = page;
            return View();
        }

        
    }
}