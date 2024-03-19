namespace CitroenAPI.Models
{
    public class Service : Enums
    {
        public bool requiredCourtesyCar { get; set; }
        public DateTime vehDropoffDate { get; set; }
        public string vehDropoffTime { get; set; }
        public DateTime vehPickupDate { get; set; }
        public string vehPickupTime { get; set; }
        public List<string> serviceTypes { get; set; }
    }
}