using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AstCaller.DataLayer;
using AstCaller.Models.Domain;
using AstCaller.ViewModels;
using AstCaller.ViewModels.Reports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AstCaller.Controllers
{
    public class ReportController : Controller
    {
        private readonly MainContext _context;
        private readonly IReportingService _reportingService;

        public ReportController(MainContext context, IReportingService reportingService)
        {
            _context = context;
            _reportingService = reportingService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Campaign(int campaignId = 0)
        {
            ViewBag.Campaigns = await GetCampaigns();
            ViewBag.CampaignId = campaignId;
            if (campaignId < 1)
            {
                return View();
            }

            var stats = _reportingService.GetCampaignStatsAsync(campaignId);
            var details = _reportingService.GetCampaignDetailedReport(campaignId);

            var model = new ReportCampaignsViewModel
            {
                Stats = await stats,
                Details = await details
            };

            return View(model);
        }

        public async Task<IActionResult> FailedCalls(int campaignId = 0)
        {
            ViewBag.Campaigns = await GetCampaigns();
            ViewBag.CampaignId = campaignId;
            if (campaignId < 1)
            {
                return View();
            }

            var stats = await _context.CampaignAbonents.Where(x => x.CampaignId == campaignId && 
                !_context.CampaignAbonentHistories.Any(h => h.CampaignAbonentId == x.Id && h.Status == 2))
                .Select(x => new ReportFailedCallsViewModel
                {
                    Id = x.Id,
                    Phone = x.Phone,
                    Status = x.CallStatus.StatusName,
                    Attempts = x.CampaignAbonentHistories.Count()
                }).ToArrayAsync();

            return View(stats);
        }

        private async Task<IEnumerable<IdNameViewModel>> GetCampaigns()
        {
            return await _context.Campaigns
                .Where(x => x.Status != (int)CampaignViewModel.CampaignStatuses.Created)
                .OrderByDescending(x => x.Id)
                .Select(x => new IdNameViewModel
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToArrayAsync();
        }
    }
}