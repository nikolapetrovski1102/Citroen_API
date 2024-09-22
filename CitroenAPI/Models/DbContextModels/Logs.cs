namespace CitroenAPI.Models.DbContextModels
{
    public class Logs
    {
        public string GitId { get; set; }
        public DateTime DispatchDate { get; set; }
        public DateTime CreatedDate { get; set; }
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