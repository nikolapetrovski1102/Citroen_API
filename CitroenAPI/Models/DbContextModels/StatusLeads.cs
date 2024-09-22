namespace CitroenAPI.Models.DbContextModels
{
    public class StatusLeads
    {
        public string GitId { get; set; }
        public int Status { get; set; }
        public DateTime SentDate { get; set; }
        public string Name { get; set; }
        public string FamilyName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string RequestType { get; set; }
        public string Model { get; set; }
        public string Dealer { get; set; }
        public string Consents { get; set; }
        public string Comments { get; set; }

    }
}