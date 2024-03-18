using static CitroenAPI.Models.Enums;

namespace CitroenAPI.Models
{
    public class Message
    {
        public string gitId { get; set; }
        public DateTime creationDate { get; set; }
        public DateTime dispatchDate { get; set; }
        public string leadStatus { get; set; }
        public string leadBatchTime { get; set; }
        public string applicationSourceCode { get; set; }
        public string applicationSourceName { get; set; }
        public LeadData leadData { get; set; }
        public PreferredContactMethodEnum preferredContactMethod { get; set; }
    }
}
