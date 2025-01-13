using System.ComponentModel.DataAnnotations;
namespace Weather_Monitoring_Application.Models
{
    public class Location
    {
        public int Id { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        [Display(Name = "Zip Code")]
        public string? Zipcode { get; set; }
    }
}
