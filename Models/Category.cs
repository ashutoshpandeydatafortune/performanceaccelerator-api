using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DF_PA_API.Models
{
    [Table("PA_Categories")]
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public byte? IsActive { get; set; }
        public int? CreateBy { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }

    }
}
