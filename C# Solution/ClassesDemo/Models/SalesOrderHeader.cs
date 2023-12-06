using System;

namespace Appeon.ComponentsApp.ClassesDemo.Models
{
    public  class SalesOrderHeader
    {
        public int SalesOrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime ShipDate { get; set; }
        public int Status { get; set; }
        public int OnlineOrderFlag { get; set; }
        public int CustomerID { get; set; }
        public int ShipToAddressID { get; set; }
    }
}
