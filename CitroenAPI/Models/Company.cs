namespace CitroenAPI.Models
{
    public class Company : Enums
    {
        public string companyName { get; set; }
        public string companySector { get; set; }
        public string activityType { get; set; }
        public string job { get; set; }
        public string registrationNumber { get; set; }
        public string professionalPhone { get; set; }
        public string professionalMobilePhone { get; set; }
        public CivilityEnum contactCivility { get; set; }
        public string contactFirstname { get; set; }
        public string contactLastname { get; set; }
        public string contactEmail { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string postalCode { get; set; }
        public string city { get; set; }
        public string department { get; set; }
        public string region { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public int fleetSize { get; set; }
        public int numberOfWorkers { get; set; }
        public string fiscalId { get; set; }
        public string companyId { get; set; }
    }
}