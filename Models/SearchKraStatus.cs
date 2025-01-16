using System;

namespace DF_PA_API.Models
{
    public class SearchKraStatus
    {
        public int FunctionId {  get; set; }
       // public int DesignationId {  get; set; }
        public int DesignatedRoleId { get; set; }
        public DateTime? FromDate { get; set;}
        public DateTime? ToDate { get; set;}
    }
}
