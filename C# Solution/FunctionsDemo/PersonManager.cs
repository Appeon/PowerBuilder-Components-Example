using Appeon.ComponentsApp.CSharpFunctions.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Appeon.ComponentsApp.CSharpFunctions
{
    public class PersonManager
    {
        private string? DataPath { get; set; }

        private const string EmployeeAddressesFile = "EmployeeAddress.json";
        private const string EmployeeQuotaFile = "EmployeeQuota.json";

        public PersonManager()
        {


        }

        [MemberNotNullWhen(true, nameof(DataPath))]
        private bool CheckPath([NotNull()] string? path = null)
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

            DataPath = path; ;
        }

        public int GetAddressDetails(int id,
            out string? addressLine1,
            out string? addressLine2,
            out string? city,
            out string? stateName,
            out string? addressType,
            out string? error)
        {
            addressLine1 = null;
            addressLine2 = null;
            city = null;
            stateName = null;
            addressType = null;
            error = null;

            try
            {
                var data = LoadDataFromFile<Address>(EmployeeAddressesFile);

                var item = data.FirstOrDefault(a => a.EmployeeId == id);

                if (item is null)
                {
                    error = "No address for such EmployeeId";
                    return -1;
                }

                addressLine1 = item.AddressLine1;
                addressLine2 = item.AddressLine2;
                city = item.City;
                stateName = item.StateProvince;
                addressType = item.Type;

            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }

            return 1;
        }

        public int GetQuotaDetails(int salesPersonId,
            out decimal? salesQuota,
            out decimal? bonus,
            out decimal? commission,
            out decimal? salesYTD,
            out decimal? salesLastYear,
            out string? error)
        {
            salesQuota = null;
            bonus = null;
            commission = null;
            salesYTD = null;
            salesLastYear = null;
            error = null;

            try
            {
                var data = LoadDataFromFile<EmployeeQuota>(EmployeeQuotaFile);
                var detail = data.FirstOrDefault(q => q.EmployeeId == salesPersonId);

                if (detail is null)
                {
                    error = "No Quota information for that EmployeeId";
                    return -1;
                }

                salesQuota = detail.SalesQuota;
                bonus = detail.Bonus;
                commission = detail.CommissionPct;
                salesYTD = detail.SalesYTD;
                salesLastYear = detail.SalesLastYear;
            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }
            return 1;
        }

        private IList<T> LoadDataFromFile<T>(string file)
        {
            var fullPath = Path.Combine(DataPath!, file);

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
