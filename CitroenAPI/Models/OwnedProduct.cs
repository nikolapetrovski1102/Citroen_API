using static CitroenAPI.Models.Enums;

namespace CitroenAPI.Models
{
    public class OwnedProduct : Enums
    {
        public bool isPSAProduct { get; set; }
        public string brand { get; set; }
        public string category { get; set; }
        public string model { get; set; }
        public string finition { get; set; }
        public string description { get; set; }
        public string lcdv { get; set; }
        public string mileage { get; set; }
        public string vin { get; set; }
        public VechicleTypeEnum vehicleType { get; set; }
        public string vehicleStatus { get; set; }
        public string vehiclePlate { get; set; }
        public string vehicleCondition { get; set; }
        public string estimatePrice { get; set; }
        public string finalePrice { get; set; }
        public string valuationPriceForVO { get; set; }
        public string fiscalTaxPower { get; set; }
        public string promotionalCode { get; set; }
        public DateTime purchaseDate { get; set; }
        public DateTime firstLicenceDate { get; set; }
        public string energyType { get; set; }
        public Service service { get; set; }
    }
}