using System.ComponentModel.DataAnnotations.Schema;

namespace DF_PA_API.Models
{
    public class ResourceKrasSatus
    {
        public int ResourceId { get; set; }
        //  public int? FunctionId { get; set; }
        public string EmployeeId { get; set; }
        public string ResourceName { get; set; }
        public string DesignationName { get; set; }
        public string QuarterName { get; set; }
        public string KraName { get; set; }
        public string DeveloperComment { get; set; }
        public string ManagerComment { get; set; }
        public double? DeveloperRating { get; set; }
        public double? ManagerRating { get; set; }
        public double? FinalRating { get; set; }
        public string FinalComment { get; set; }
        //   public double Score { get; set; }
        public int? Status { get; set; }
        // public int? UserId { get; set; }
        public int? KRAId { get; set; }
        public int? QuarterId { get; set; }
        //   public int? ApprovedBy { get; set; }
        public int? RejectedBy { get; set; }
        public string Reason { get; set; }
        public string Comment { get; set; }
        [NotMapped]
        //public string KRAName { get; set; }
        // public int? AppraisalRange { get; set; }
        public byte? IsActive { get; set; }
        public byte? IsDeleted { get; set; }

    }

    public class FunctionsDesignations
    {
        public int FunctionId { get; set; }
        public int DesignationId { get; set; }
        public string FunctionName { get; set; }
        public string DesignationName { get; set; }
    }
}
