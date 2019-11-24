using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AstCaller.Models.Domain;
using AstCaller.Services;
using AstCaller.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AstCaller.Controllers
{
    public class ExtensionController : BaseAuthorizedController
    {
        private MainContext _context;

        public ExtensionController(ILogger<BaseAuthorizedController> logger, IUserProvider userProvider, MainContext context) : base(logger, userProvider)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var extensions = await _context.AsteriskExtensions.Select(x => new AsteriskExtensionViewModel
            {
                Extension = x.Extension,
                Title = x.Title,
                Disabled = x.Disabled
            }).ToArrayAsync();

            return View(extensions);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string extension = "")
        {
            var model = new AsteriskExtensionViewModel
            {
            };

            if (!string.IsNullOrEmpty(extension))
            {
                var entity = await _context.AsteriskExtensions.FirstOrDefaultAsync(x => x.Extension == extension);

                model.Extension = entity.Extension;
                model.Title = entity.Title;
                model.ExtensionCode = entity.ExtensionCode;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AsteriskExtensionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var entity = await _context.AsteriskExtensions.FirstOrDefaultAsync(x => x.Extension == model.Extension);
            if (entity == null)
            {
                entity = new AsteriskExtension
                {
                    Extension = model.Extension
                };

                _context.AsteriskExtensions.Add(entity);
            }

            entity.ModifierId = _currentUserId.Value;
            entity.Title = model.Title;
            entity.ExtensionCode = model.ExtensionCode;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Disable(string extension)
        {
            var entity = await _context.AsteriskExtensions.FirstAsync(x => x.Extension == extension);
            entity.Disabled = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}