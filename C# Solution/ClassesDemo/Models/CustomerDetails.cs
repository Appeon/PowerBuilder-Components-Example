using System.Text.Json.Serialization;

namespace Appeon.ComponentsApp.ClassesDemo.Models
{
    public class CustomerDetails
    {
        [JsonPropertyName("CustomerID")]
        public int Id { get; set; }
        public string? AccountNumber { get; set; }
        public string? PersonType { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Territory { get; set; }
        public string? CountryCode { get; set; }
    }
}
