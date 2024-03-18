namespace CitroenAPI.Models
{
    public class LeadData : Enums
    {

        public LeadTypeEnum leadType { get; set; }
        public string leadLabel { get; set; }
        public DeviceTypeEnum deviceType { get; set; }
        public string gaClientId { get; set; }
        public string userAgent { get; set; }
        public RequestTypeEnum requestType { get; set; }
        public CustomerTypeEnum customerType { get; set; }
        public BrandEnum brand { get; set; }
        public string country { get; set; }
        public string language { get; set; }
        public string isOMNI { get; set; }
        public ActivityEnum activity { get; set; }
        public string marketingCode { get; set; }
        public Customer customer { get; set; }
        public InterestProduct interestProduct { get; set; }
        public List<Dealer> dealers { get; set; }
        public List<Consent> consents { get; set; }
        public List<OtherField> otherFields { get; set; }
    }
}
