using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AstCaller.Models.Domain;
using AstCaller.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AstCaller.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly MainContext _context;
        private readonly IConfiguration _configuration;

        public StatisticsController(MainContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IActionResult> Current()
        {
            var timeShift = _configuration.GetValue<double>("Host:TimeShift");
            var now = DateTime.Now.AddHours(timeShift);
            var nowTime = (int)(now.TimeOfDay.TotalSeconds / 60);
            var nowDay = (int)Math.Pow(2, (int)now.DayOfWeek);

            var model = new CurrentStatisticsViewModel
            {
                ActiveCampaigns = await _context.Campaigns.Where(x => x.Status == (int)CampaignViewModel.CampaignStatuses.Running).CountAsync(),
                CurrentRunningCampaigns = await _context.Campaigns.Where(x => x.Status == (int)CampaignViewModel.CampaignStatuses.Running &&
                    x.CampaignSchedules.Any(s =>
                            s.DateStart < now &&
                            s.DateEnd.AddDays(1) > now &&
                            s.TimeStart < nowTime &&
                            s.TimeEnd > nowTime &&
                            (s.DaysOfWeek & nowDay) > 0)).CountAsync(),
                AbonentsInProcess = await _context.CampaignAbonents.Where(x => x.Status == 1).CountAsync(),
                AbonentsInProcessStuck = await _context.CampaignAbonents.Where(x => x.Status == 1 && (!x.CallStartDate.HasValue || x.CallStartDate < now.AddMinutes(120)) && !x.CampaignAbonentHistories.Any()).CountAsync(),
                AbonentsToCall = await _context.CampaignAbonents.Where(x => x.Status == 0 && x.Campaign.Status == (int)CampaignViewModel.CampaignStatuses.Running).CountAsync()
            };

            return Json(model);
        }

        public async Task<IActionResult> Hourly(DateTime? currentDate = null)
        {
            var timeShift = _configuration.GetValue<double>("Host:TimeShift");
            if (!currentDate.HasValue)
            {
                currentDate = DateTime.Today;
            }
            var today = currentDate.Value.AddHours(-timeShift);


            var query = from h in _context.CampaignAbonentHistories
                        join a in _context.CampaignAbonents on h.CampaignAbonentId equals a.Id
                        where h.CallDate >= today && h.CallDate < today.AddHours(24)
                        select new
                        {
                            currentHour = h.CallDate.AddHours(timeShift).Hour,
                            h.Status
                        };

            var res = await query.GroupBy(k => new { k.currentHour, k.Status }, (k, g) => new
            {
                k.currentHour,
                k.Status,
                Cnt = g.Count()
            }).ToArrayAsync();

            return Json(new HourlyStatisticsViewModel
            {
                Statuses = await _context.CallStatuses.ToDictionaryAsync(x => x.Id, x => x.StatusName),
                Hours = res.GroupBy(
                    k => k.currentHour,
                    (k, g) => new HourlyStatisticsValueViewModel
                    {
                        Hour = k,
                        Counts = g.GroupBy(hgroup => hgroup.Status)
                            .ToDictionary(x => x.Key ?? 0, x => x.Sum(_ => _.Cnt))
                    }).OrderBy(x => x.Hour).ToDictionary(k=>k.Hour,k=>k)
            });
        }
    }
}