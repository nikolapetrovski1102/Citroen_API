namespace CitroenAPI.Models
{
    public class LeadData
    {
        public string leadType { get; set; }
        public string leadLabel { get; set; }
        public string deviceType { get; set; }
        public string gaClientId { get; set; }
        public string userAgent { get; set; }
        public string requestType { get; set; }
        public string customerType { get; set; }
        public string brand { get; set; }
        public string country { get; set; }
        public string language { get; set; }
        public string isOMNI { get; set; }
        public string activity { get; set; }
        public string marketingCode { get; set; }
        public Customer customer { get; set; }
        public InterestProduct interestProduct { get; set; }
        public List<Dealer> dealers { get; set; }
        public List<Consent> consents { get; set; }
        public List<OtherField> otherFields { get; set; }
    }
}
