using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AstCaller.Classes;
using AstCaller.Models;
using AstCaller.Models.Domain;
using AstCaller.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AstCaller.Services.Implementations
{
    public class ScheduleService : IScheduleService
    {
        private readonly IContextProvider _contextProvider;

        public ScheduleService(IContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public async Task<IEnumerable<ScheduleTaskModel>> GetCurrentSchedulesAsync()
        {
            using(var context = _contextProvider.GetContext())
            {
                var now = DateTime.Now;
                var nowTime = now.TimeOfDay.TotalSeconds;
                var nowDay = (int)Math.Pow(2, (int)now.DayOfWeek);
                var res = context.Campaigns.Where(x => x.DateStart < now &&
                                        x.DateEnd > now && 
                                        x.TimeStart < nowTime &&
                                        x.TimeEnd > nowTime &&
                                        (x.DaysOfWeek & nowDay) == 1 &&
                                        x.Status == (int)CampaignViewModel.CampaignStatuses.Running);

                return await res.Select(x => new ScheduleTaskModel
                {
                    CampaignId = x.Id
                }).ToArrayAsync();
            }
        }
    }
}
