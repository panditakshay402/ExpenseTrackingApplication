using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.ViewModels
{
    public class ReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        // List of transactions for the report
        public List<Transaction>? Transactions { get; set; }
    }
}