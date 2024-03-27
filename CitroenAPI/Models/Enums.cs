using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Field)]
public class EnumStringValueAttribute : Attribute
{
    public string Value { get; }

    public EnumStringValueAttribute(string value)
    {
        Value = value;
    }
}

namespace CitroenAPI.Models
{
    public class Enums
    {

        public static string GetEnumValue<T>(T enumValue) where T : Enum
        {
            Type enumType = typeof(T);
            FieldInfo fieldInfo = enumType.GetField(enumValue.ToString());
            var attribute = fieldInfo?.GetCustomAttribute<EnumStringValueAttribute>();
            if (attribute != null)
            {
                return attribute.Value;
            }
            else
            {
                return "--None--";
            }
        }

        public enum BrandEnum
        {
            [EnumStringValue("Peugeot")]
            AP,
            [EnumStringValue("Citroën")]
            AC,
            [EnumStringValue("DS")]
            DS,
            [EnumStringValue("Opel")]
            OP,
            [EnumStringValue("Vauxhall")]
            VX,
            [EnumStringValue("FCA")]
            FCA
        }

        public enum LeadTypeEnum
        {
            [EnumStringValue("Lead")]
            LEAD,
            [EnumStringValue("RLC")]
            RLC,
            [EnumStringValue("NWL")]
            NWL,
            [EnumStringValue("other_rooting")]
            other_rooting = 4,
            [EnumStringValue("ORHER")]
            ORHER = 9
        }

        public enum DeviceTypeEnum
        {
            [EnumStringValue("Desktop")]
            DESKTOP,
            [EnumStringValue("Mobile")]
            MOBILE,
            [EnumStringValue("Tablet")]
            TABLET,
            [EnumStringValue("Other")]
            OTHER = 9,
        }

        public enum ActivityEnum
        {
            [EnumStringValue("New vehicle")]
            VN,
            [EnumStringValue("Used cars")]
            VO,
            [EnumStringValue("After sale")]
            APV
        }

        public enum CustomerTypeEnum
        {
            [EnumStringValue("B2C")]
            B2C,
            [EnumStringValue("B2B")]
            B2B,
            [EnumStringValue("Cold Prospect")]
            Cold_Prospect
        }

        public enum VechicleTypeEnum
        {
            [EnumStringValue("Particular vehicle")]
            VP,
            [EnumStringValue("Commercial vehicle")]
            VU
        }

        public enum TitleEnum
        {
            [EnumStringValue("Master")]
            MAS,
            [EnumStringValue("Director")]
            DR,
            [EnumStringValue("Professor")]
            PRO,
            [EnumStringValue("Doctor")]
            DOC,
            [EnumStringValue("Judge")]
            JUD,
            [EnumStringValue("Lawyer")]
            LAW,
            [EnumStringValue("OTH")]
            OTH,
            [EnumStringValue("DEN")]
            DEN,
            [EnumStringValue("ENG")]
            ENG,
            [EnumStringValue("PRD")]
            PRD,
            [EnumStringValue("LAD")]
            LAD,
            [EnumStringValue("LRD")]
            LRD,
            [EnumStringValue("SIR")]
            SIR,
            [EnumStringValue("REV")]
            REV
        }

        public enum PreferredContactMethodEnum
        {
            [EnumStringValue("By Email")]
            Email,
            [EnumStringValue("By Phone call")]
            Phone,
            [EnumStringValue("By SMS")]
            SMS
        }

        public enum RequestTypeEnum
        {
            [EnumStringValue("--None--")]
            None=0,
            [EnumStringValue("Test Drive")]
            TestDrive = 1,
            [EnumStringValue("Request Offer")]
            Offer = 2,
            [EnumStringValue("Contact Forms")]
            Contact = 4,
            [EnumStringValue("Prelead – keep me informed")]
            PreLead = 10,
        }

        public enum CivilityEnum
        {
            [EnumStringValue("--None--")]
            None=0,
            [EnumStringValue("Mr.")]
            MR = 1,
            [EnumStringValue("Mrs.")]
            MRS = 2,
            [EnumStringValue("Miss.")]
            MISS = 3,
            [EnumStringValue("Ms.")]
            MS = 4,
            [EnumStringValue("Dr.")]
            DR=5,
            [EnumStringValue("Prof.")]
            PROF=6

        }

        public enum PurchaseIntentionPeriodEnum
        {
            [EnumStringValue("--None--")]
            None=0,
            [EnumStringValue("less than 3 months")]
            LessThan3Months = 1,
            [EnumStringValue("between 3 & 6 months")]
            Between3And6Months = 2,
            [EnumStringValue("> 6 months")]
            MoreThan6Months = 3,
            [EnumStringValue("no intention of purchase")]
            NoIntentionOfPurchase = 4
        }

        public enum ConsentNameEnum
        {
            [EnumStringValue("By Postal")]
            consentMail,
            [EnumStringValue("By Email")]
            consentEmail,
            [EnumStringValue("By Phone call")]
            consentPhone,
            [EnumStringValue("By SMS message")]
            consentSMS
        }

        public enum FleetSizeEnum
        {
            [EnumStringValue("less than 5")]
            LessThan5 = 5,
            [EnumStringValue("between 6 & 10")]
            Between6And10 = 10,
            [EnumStringValue("between 11 & 20")]
            Between11And20 = 20,
            [EnumStringValue("between 21 & 50")]
            Between21And50 = 50,
            [EnumStringValue("between 51 & 100")]
            Between51And100 = 100,
            [EnumStringValue("between 101 & 250")]
            Between101And250 = 250,
            [EnumStringValue("more than 251")]
            MoreThan251 = 500
        }

    }
}