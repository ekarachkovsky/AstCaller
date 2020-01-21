using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AstCaller.Models.Domain;
using AstCaller.Services;
using AstCaller.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AstCaller.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : BaseAuthorizedController
    {
        private readonly MainContext _context;

        public UsersController(ILogger<BaseAuthorizedController> logger, IUserProvider userProvider, MainContext context) : base(logger, userProvider)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _context.Users.Select(x => new UserViewModel
            {
                Id = x.Id,
                Login = x.Login,
                Fullname = x.Fullname,
                Role = x.Role
            }).ToArrayAsync();

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id = null)
        {
            var model = new UserViewModel();

            if (id.HasValue)
            {
                var entity = await _context.Users.FirstAsync(x => x.Id == id.Value);
                model.Id = entity.Id;
                model.Fullname = entity.Fullname;
                model.Login = entity.Login;
                model.Role = entity.Role;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var entity = new User();

                if (model.Id.HasValue)
                {
                    entity = await _context.Users.FirstAsync(x => x.Id == model.Id);
                }
                else
                {
                    if (string.IsNullOrEmpty(model.Password))
                    {
                        ModelState.AddModelError(nameof(model.Password), "Поле Пароль обязательно для заполнения!");
                        return View(model);
                    }
                }

                entity.Login = model.Login;
                entity.Fullname = model.Fullname;
                entity.Role = model.Role;
                if (!string.IsNullOrEmpty(model.Password))
                {
                    entity.Password = model.Password;
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not save Campaign");
                ViewBag.Error = ex.Message;
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Users.FirstAsync(x => x.Id == id);
            entity.IsDeleted = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}