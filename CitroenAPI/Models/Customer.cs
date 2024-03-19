namespace CitroenAPI.Models
{
    public class Customer : Enums
    {
        public CivilityEnum civility { get; set; }
        public string title { get; set; }
        public string firstname { get; set; }
        public string middlename { get; set; }
        public string lastname { get; set; }
        public DateTime birthDate { get; set; }
        public string email { get; set; }
        public string personalPhone { get; set; }
        public string personalMobilePhone { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string postalCode { get; set; }
        public string city { get; set; }
        public string department { get; set; }
        public string region { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string fiscalId { get; set; }
        public string customerId { get; set; }
        public string disclaimerId { get; set; }
    }
}
