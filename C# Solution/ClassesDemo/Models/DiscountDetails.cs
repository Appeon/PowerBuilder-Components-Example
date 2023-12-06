using System;

namespace Appeon.ComponentsApp.ClassesDemo.Models
{
    public class DiscountDetails
    {
        public int SalesOrderDetailID { get; set; }
        public int SpecialOfferID { get; set; }
        public string? Description { get; set; }
        public string? DiscountPct { get; set; }
        public string? Type { get; set; }
        public string? Category { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MinQty { get; set; }
        public object? MaxQty { get; set; }
        public string? rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
