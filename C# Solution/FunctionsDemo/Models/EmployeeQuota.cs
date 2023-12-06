namespace Appeon.ComponentsApp.CSharpFunctions.Models
{
    public class EmployeeQuota
    {
        public int EmployeeId { get; set; }
        public decimal? SalesQuota { get; set; }
        public decimal SalesYTD { get; set; }
        public decimal SalesLastYear { get; set; }
        public decimal Bonus { get; set; }
        public decimal CommissionPct { get; set; }
    }
}
