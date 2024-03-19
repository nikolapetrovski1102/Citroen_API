﻿using System;
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
            var attribute = fieldInfo.GetCustomAttribute<EnumStringValueAttribute>();
            if (attribute != null)
            {
                return attribute.Value;
            }
            else
            {
                throw new InvalidOperationException($"EnumStringValueAttribute not found for {enumValue.ToString()}.");
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
            [EnumStringValue("TESTDRIVE")]
            TestDrive = 1,
            [EnumStringValue("OFFER")]
            Offer = 2,
            [EnumStringValue("BROCHURE")]
            Brochure = 3,
            [EnumStringValue("CONTACT")]
            Contact = 4,
            [EnumStringValue("APPOINTMENT")]
            Appointment = 6,
            [EnumStringValue("AFTERSALES")]
            AfterSales = 7,
            [EnumStringValue("VN - CONTACT_PREACCEPT_POSITIVE")]
            VNContactPreAcceptPositive = 8,
            [EnumStringValue("PRELEAD")]
            PreLead = 10,
            [EnumStringValue("Contact Finance")]
            ContactFinance = 12,
            [EnumStringValue("VO - Information request")]
            VOInformationRequest = 13,
            [EnumStringValue("Reservation request")]
            ReservationRequest = 14,
            [EnumStringValue("Buyback request")]
            BuybackRequest = 15,
            [EnumStringValue("APV - Quote Request")]
            APVQuoteRequest = 32,
            [EnumStringValue("NDVO")]
            NDVO = 33,
            [EnumStringValue("NDVO finance")]
            NDVOFinance = 38,
            [EnumStringValue("Quote for Motability")]
            QuoteForMotability = 40,
            [EnumStringValue("VN - ONLINE_SALE_STOCK")]
            VNOnlineSaleStock = 45,
            [EnumStringValue("VO - VN Trade-In Operation")]
            VOVNTradeInOperation = 47,
            [EnumStringValue("CONFIG")]
            Config = 48,
            [EnumStringValue("VO - Trade-In Only Request")]
            VOTradeInOnlyRequest = 58,
            [EnumStringValue("F2ML offre")]
            F2MLOffer = 60,
            [EnumStringValue("F2ML contact")]
            F2MLContact = 61,
            [EnumStringValue("F2ML solution")]
            F2MLSolution = 62,
            [EnumStringValue("VO - Reserved with fingerprint")]
            VOReservedWithFingerprint = 68,
            [EnumStringValue("ONLINE SALE ON CONFIGURATION")]
            OnlineSaleOnConfiguration = 71,
            [EnumStringValue("SUBSCRIBE NL")]
            SubscribeNL = 73,
            [EnumStringValue("Request for quote due VO unavailability")]
            RequestForQuoteDueVOUnavailability = 77,
            [EnumStringValue("Request for reservation of a used vehicle")]
            RequestForReservationOfAUsedVehicle = 79,
            [EnumStringValue("E-SELLER")]
            ESeller = 102,
            [EnumStringValue("OTHERS")]
            Others = 200,
            [EnumStringValue("Test drive for Motability")]
            TestDriveForMotability = 301,
            [EnumStringValue("lead de commande en ligne (SPOTICAR)")]
            LeadDeCommandeEnLigneSPOTICAR = 59,
            [EnumStringValue("lead de commande en ligne (SPOTICAR)")]
            LeadDeCommandeEnLigneSPOTICAR_A74 = 74
        }

        public enum CivilityEnum
        {
            [EnumStringValue("MR")]
            MR = 1,
            [EnumStringValue("MRS")]
            MRS = 2,
            [EnumStringValue("MISS")]
            MISS = 3,
            [EnumStringValue("MS")]
            MS = 4
        }

        public enum PurchaseIntentionPeriodEnum
        {
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