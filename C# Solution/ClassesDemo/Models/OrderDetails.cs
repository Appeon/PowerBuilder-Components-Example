namespace Appeon.ComponentsApp.ClassesDemo.Models
{
    public class OrderDetails
    {
        public int SalesOrderID { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? Freight { get; set; }
        public decimal? TaxAmt { get; set; }
        public decimal? TotalDue { get; set; }
    }
}
