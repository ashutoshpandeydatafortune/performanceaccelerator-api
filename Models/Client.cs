using System;

namespace DF_EvolutionAPI.Models
{
    public class Client: BaseEntity_PRMS
    {
        public int? ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientCode { get; set; }
        public string Description { get; set; }
        public byte IsActive { get; set; }
        public int? StatusId { get; set; }

        #region Address
        public string Address { get; set; }
        public string CountryId { get; set; }
        public int? CityId { get; set; }
        public int? StateId { get; set; }
        public string Zip { get; set; }
        public string ContactNumber { get; set; }
        public string EmailId { get; set; }
        #endregion

        #region Billing
        public string Remarks { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public decimal? MonthlyBillingRate { get; set; }
        public decimal? WeeklyBillingRate { get; set; }
        public decimal? HourlyBillingRate { get; set; }
        public int? CurrencyId { get; set; }
        #endregion

        public string ClientCodeReference { get; set; }
        public string CompanyURL { get; set; }
        public decimal? ApproxRevenue { get; set; }
        public int? ApproxNoOfPeople { get; set; }
        public int? DomainId { get; set; }
        public int? CompanyTypeId { get; set; }
        public string CompanyFoundedIn { get; set; }
        public int? NoOfYearsInBusiness { get; set; }
        public int? BusinessUnitId { get; set; }
        public int? PracticeId { get; set; }
        public int? PrimaryTechnologyId { get; set; }
        public DateTime? AgreementSignedDate { get; set; }
        public DateTime? ApproxStartDate { get; set; }
        public string AgreementSignedByClient { get; set; }
        public string AgreementSignedByDF { get; set; }
        public decimal? AgreementValue { get; set; }
        public string AgreementDuration { get; set; }
        public int? SalesPersonId { get; set; }
        public int? BDEId { get; set; }
        public int? PaymentMethodId { get; set; }
        public int? PaymentCycleId { get; set; }
        public int? DeliveryManagerId { get; set; }

    }
}
