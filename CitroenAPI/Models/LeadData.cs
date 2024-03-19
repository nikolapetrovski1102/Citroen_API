namespace CitroenAPI.Models
{
    public class LeadData : Enums
    {
        public string gitId { get; set; }
        public LeadTypeEnum leadType { get; set; }
        public string leadLabel { get; set; }
        public DeviceTypeEnum deviceType { get; set; }
        public string ipAddress { get; set; }
        public string userAgent { get; set; }
        public RequestTypeEnum requestType { get; set; }
        public CustomerTypeEnum customerType { get; set; }
        public Customer customer { get; set; }
        public BrandEnum brand { get; set; }
        public string country { get; set; }
        public string language { get; set; }
        public ActivityEnum activity { get; set; }
        public string complementSource { get; set; }
        public string marketingCode { get; set; }
        public string customerEmailEventCode { get; set; }
        public string idCookies { get; set; }
        public bool flagMQ { get; set; }
        public string gaClientId { get; set; }
        public string gaUserId { get; set; }
        public DateTime appointmentDesiredDate { get; set; }
        public TimeSpan appointmentDesiredTime { get; set; }
        public string firstDesiredSchedule { get; set; }
        public PreferredContactMethodEnum preferredContactMethod { get; set; }
        public DateTime preferredRecallDate { get; set; }
        public string purchaseIntentionPeriod { get; set; }
        public string comments { get; set; }
        public string position { get; set; }
        public Company company { get; set; } 
        public OwnedProduct ownedProduct { get; set; } 
        public InterestProduct interestProduct { get; set; }
        public List<Dealer> dealers { get; set; }
        public List<Consent> consents { get; set; }
        public CampaignUtm campaignUtm { get; set; } 
        public string customerEmail { get; set; }
        public List<OtherField> otherFields { get; set; }
    }

  
}
