using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AstCaller.Models;
using AstCaller.Models.Domain;
using AstCaller.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AstCaller.Services.Implementations
{
    public class ScheduleService : IScheduleService
    {
        private readonly MainContext _context;
        private readonly IConfiguration _configuration;

        public ScheduleService(MainContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IEnumerable<ScheduleTaskModel>> GetCurrentSchedulesAsync()
        {
            var shift = _configuration.GetValue<double>("Host:TimeShift");
            var now = DateTime.Now.AddHours(shift);
            var nowTime = (int)(now.TimeOfDay.TotalSeconds/60);
            var nowDay = (int)Math.Pow(2, (int)now.DayOfWeek);

            var res = from cmp in _context.Campaigns
                      join sch in _context.CampaignSchedules on cmp.Id equals sch.CampaignId
                      where cmp.Status == (int)CampaignViewModel.CampaignStatuses.Running &&
                            sch.DateStart < now &&
                            sch.DateEnd.AddDays(1) > now &&
                            sch.TimeStart < nowTime &&
                            sch.TimeEnd > nowTime &&
                            (sch.DaysOfWeek & nowDay)>0
                      select new ScheduleTaskModel
                      {
                          CampaignId = cmp.Id,
                          LineLimit = cmp.LineLimit,
                          Action = cmp.Extension
                      };

            return await res.Distinct().ToArrayAsync();
        }
    }
}
