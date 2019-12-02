using AstCaller.ViewModels.Reports;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AstCaller.DataLayer
{
    public interface IReportingService
    {
        Task<ReportCampaignStatsViewModel> GetCampaignStatsAsync(int campaignId);

        Task<IEnumerable<ReportCampaignDetailViewModel>> GetCampaignDetailedReport(int campaignId);
    }
}
