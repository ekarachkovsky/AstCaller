using AstCaller.Models.Domain;
using AstCaller.ViewModels.Reports;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AstCaller.DataLayer.Implementations
{
    public class ReportingService : IReportingService
    {
        private readonly MainContext _context;

        public ReportingService(MainContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReportCampaignDetailViewModel>> GetCampaignDetailedReport(int campaignId)
        {
            var query = await (from ab in _context.CampaignAbonents
                        join h in _context.CampaignAbonentHistories on ab.Id equals h.CampaignAbonentId into htmp
                        from h in htmp.DefaultIfEmpty()
                        select new
                        {
                            ab.Id,
                            ab.Phone,
                            LastStatus = ab.Status,
                            LastStatusName = ab.CallStatus == null ? "n/a" : ab.CallStatus.StatusName,
                            ab.CallStartDate,
                            CallDate = h == null ? (DateTime?)null : h.CallDate,
                            Status = h==null ? (int?)null : h.Status,
                            StatusName = h==null || h.CallStatus == null ? "n/a" : h.CallStatus.StatusName,
                            Reason = h==null ? "" : h.Reason
                        }).ToArrayAsync();

            return query.GroupBy(k => new { k.Id, k.Phone, k.LastStatus, k.LastStatusName, k.CallStartDate },
                (k, g) => new ReportCampaignDetailViewModel
                {
                    Id = k.Id,
                    Phone = k.Phone,
                    LastStatus = k.LastStatus,
                    LastStatusName = k.LastStatusName,
                    CallStartDate = k.CallStartDate,
                    Attempts = g.Where(h=>h.CallDate.HasValue).Select(h => new ReportCampaignDetailAttemptViewModel
                    {
                        CallDate = h.CallDate.Value,
                        Status = h.Status,
                        StatusName = h.StatusName,
                        Reason = h.Reason
                    }).ToArray()
                }).ToArray();
        }

        public async Task<ReportCampaignStatsViewModel> GetCampaignStatsAsync(int campaignId)
        {
            var query = await _context.SqlQueryAsync<ReportCampaignStatsViewModel>($@"
    select count(distinct ab.id) TotalAbonents, 
            count(distinct case when ab.status = 2 then ab.Id end) Answered,
            count(distinct case when ab.status = 3 then ab.Id end) UnansweredAbonents,
            count(distinct case when ab.status = 1 then ab.Id end) InProcess,
            count(distinct case when h.status = 3 then h.Id end) TotalUnansweredCalls,
            count(distinct case when ab.haserrors = 1 then ab.Id end) TotalAbonentsWithErrors
    from campaignabonent ab
    left join campaignabonenthistory h on ab.id=h.campaignabonentid
where campaignid={campaignId}
");
            return query.First();
        }
    }
}
