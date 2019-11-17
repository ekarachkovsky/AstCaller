using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AstCaller.Models;
using AstCaller.Models.Domain;
using AstCaller.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AstCaller.Services.Implementations
{
    public class ScheduleService : IScheduleService
    {
        private readonly MainContext _context;

        public ScheduleService(MainContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ScheduleTaskModel>> GetCurrentSchedulesAsync()
        {
            var now = DateTime.Now;
            var nowTime = (int)(now.TimeOfDay.TotalSeconds/60);
            var nowDay = (int)Math.Pow(2, (int)now.DayOfWeek);

            var res = from cmp in _context.Campaigns
                      join sch in _context.CampaignSchedules on cmp.Id equals sch.CampaignId
                      where cmp.Status == (int)CampaignViewModel.CampaignStatuses.Running &&
                            sch.DateStart < now &&
                            sch.DateEnd > now &&
                            sch.TimeStart < nowTime &&
                            sch.TimeEnd > nowTime &&
                            (sch.DaysOfWeek & nowDay)>0
                      select new ScheduleTaskModel
                      {
                          CampaignId = cmp.Id
                      };

            return await res.Distinct().ToArrayAsync();
        }
    }
}
