using static CitroenAPI.Models.Enums;

namespace CitroenAPI.Models
{
    public class InterestProduct : Enums
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
        public string configurationLink { get; set; }
        public string configurationImageLink { get; set; }
        public string configurationDescription { get; set; }
        public string estimatePrice { get; set; }
        public string finalePrice { get; set; }
        public string valuationPriceForVO { get; set; }
        public string fiscalTaxPower { get; set; }
        public string promotionalCode { get; set; }
        public DateTime initialPurchaseDate { get; set; }
        public string financeDescription { get; set; }
        public string financeContractNumber { get; set; }
        public decimal financedAmount { get; set; }
        public int financeDuration { get; set; }
        public decimal monthlyPayments { get; set; }
        public DateTime endOfContractDate { get; set; }
        public int remainingAmount { get; set; }
        public int numberOffleetVehiclesOnTheFinanceContract { get; set; }
        public string mileageBeforeCheckOut { get; set; }
        public string energyType { get; set; }
        public int daysNumberBeforeCheckOut { get; set; }
        public string engine { get; set; }
        public string priceType { get; set; }
        public int leasingDuration { get; set; }
        public string voVIN { get; set; }
        public string voRegistrationNumber { get; set; }
        public DateTime voRegistrationDate { get; set; }
        public decimal voSalePrice { get; set; }
        public string voGearboxType { get; set; }
        public string voEnergy { get; set; }
        public int voMileage { get; set; }
        public string voAnnouncementReference { get; set; }
        public string voAnnouncementLink { get; set; }
        public string voWarrantyType { get; set; }
        public int voWarrantyDuration { get; set; }
        public string mapId { get; set; }
        public string service { get; set; }
        public List<string> brochuresList { get; set; }
    }
}

