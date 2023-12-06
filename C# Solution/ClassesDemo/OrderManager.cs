using Appeon.ComponentsApp.ClassesDemo.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Appeon.ComponentsApp.ClassesDemo
{
    public class OrderManager
    {
        public string? DataPath { get; set; }
        private const string SalesOrderHeaderFile = "SalesOrderHeader.json";
        private const string ShippingDetailsFile = "ShippingDetails.json";
        private const string DiscountDetailsFile = "DiscountDetails.json";
        private const string CustomerDetailsFile = "CustomerDetails.json";
        private const string ProductOrderDetailFle = "ProductOrderDetails.json";
        private const string SalesOrderDetailsFile = "OrderDetails.json";


        public OrderManager()
        {

        }


        [MemberNotNullWhen(true, nameof(DataPath))]
        private bool CheckPath([NotNull] string? path = null)
        {
            path ??= DataPath;

            if (path is null)
            {
                throw new InvalidOperationException("Data lookup path not specified");
            }

            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException("Data lookup path is not a valid directory");
            }

#pragma warning disable CS8775 // Member must have a non-null value when exiting in some condition.
            return true;
#pragma warning restore CS8775 // Member must have a non-null value when exiting in some condition.
        }

        [MemberNotNull(nameof(DataPath))]
        public void Init(string path)
        {
            CheckPath(path);

            DataPath = path;

        }


        public int GetHeader(int orderId, out SalesOrderHeader? header, out string? error)
        {
            header = null;
            error = null;

            try
            {
                var customers = LoadDataFromFile<SalesOrderHeader>(SalesOrderHeaderFile);

                header = customers.First(c => c.SalesOrderID == orderId);
                return 1;
            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }
        }

        public int RetrieveDetailsForOrder(int orderId, out string? salesOrderDetail, out string? error)
        {
            salesOrderDetail = null;
            error = null;
            var path = Path.Combine(DataPath, ProductOrderDetailFle);
            try
            {
                using var stream = File.OpenRead(path);
                var data = JsonSerializer.Deserialize<IList<OrderProductDetail>>(stream);
                if (data is null)
                {
                    error = "Reading file yielded no results";
                    return -3;
                }

                salesOrderDetail = JsonSerializer.Serialize(data.Where(data => data.SalesOrderID == orderId).ToArray());
            }
            catch (FileNotFoundException)
            {
                error = $"File [{path}] does not exist";
                return -1;
            }
            catch (Exception e)
            {
                error = e.Message;
                return -2;
            }
            return 1;
        }

        public int GetCustomerDetails(int id, out CustomerDetails? details, out string? error)
        {
            details = null;
            error = null;

            try
            {
                var customers = LoadDataFromFile<CustomerDetails>(CustomerDetailsFile);

                details = customers.First(c => c.Id == id);
                return 1;
            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }
        }

        public int GetShippingDetails(int id, out AddressDetails? details, out string? error)
        {
            details = null;
            error = null;

            try
            {
                var customers = LoadDataFromFile<AddressDetails>(ShippingDetailsFile);

                details = customers.First(c => c.AddressID == id);
                return 1;
            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }
        }

        public int GetOrderDetails(int id, out OrderDetails? details, out string? error)
        {
            details = null;
            error = null;

            try
            {
                var customers = LoadDataFromFile<OrderDetails>(SalesOrderDetailsFile);

                details = customers.First(c => c.SalesOrderID == id);
                return 1;
            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }
        }

        public int GetDiscountDetails(int id, out DiscountDetails? details, out string? error)
        {
            details = null;
            error = null;

            try
            {
                var customers = LoadDataFromFile<DiscountDetails>(DiscountDetailsFile);

                details = customers.First(c => c.SalesOrderDetailID == id);
                return 1;
            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }
        }

        private IList<T> LoadDataFromFile<T>(string file)
        {
            var fullPath = Path.Combine(DataPath, file);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException("Specified path doesn't exist", fullPath);
            }

            using var stream = File.OpenRead(fullPath);
            var list = JsonSerializer.Deserialize<IList<T>>(stream)
                ?? throw new InvalidOperationException($"Could not deserialize file {fullPath}");
            return list;
        }
    }
}
