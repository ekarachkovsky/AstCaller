namespace AstCaller.ViewModels.Reports
{
    public class ReportCampaignStatsViewModel
    {
        public int TotalAbonents { get; set; }

        public int Answered { get; set; }
        
        public int UnansweredAbonents { get; set; }
        
        public int InProcess { get; set; }
        
        public int TotalUnansweredCalls { get; set; }
        
        public int TotalAbonentsWithErrors { get; set; }

        public int SentToQueue { get; set; }

        public int AnsweredByOperator { get; set; }
    }
}
