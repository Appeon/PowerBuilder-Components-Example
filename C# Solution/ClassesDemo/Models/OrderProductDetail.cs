namespace Appeon.ComponentsApp.ClassesDemo.Models
{
    public class OrderProductDetail
    {
        public int? ProductID { get; set; }
        public string? Name { get; set; }
        public string? ProductNumber { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public decimal? ListPrice { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? UnitPriceDiscount { get; set; }
        public int? OrderQty { get; set; }
        public double? LineTotal { get; set; }
        public int? SalesOrderDetailID { get; set; }
        public int? SalesOrderID { get; set; }
    }
}
