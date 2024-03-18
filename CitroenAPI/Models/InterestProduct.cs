namespace CitroenAPI.Models
{
    public class InterestProduct : Enums
    {
        public string lcdv { get; set; }
        public VechicleTypeEnum vehicleType { get; set; }
        public List<object> brochuresList { get; set; }
        public string description { get; set; }

        public string model { get; set; }
    }
}
