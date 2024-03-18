namespace CitroenAPI.Models
{
    public class Customer : Enums
    {
        public CivilityEnum civility { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string personalMobilePhone { get; set; }
    }
}