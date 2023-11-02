using System;
using System.ComponentModel.DataAnnotations;

namespace DF_EvolutionAPI.Models
{
    public class BaseEntity
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int CreateBy { get; set; }

        [Required]
        public int UpdateBy { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        [Required]
        public DateTime UpdateDate { get; set; }

    }
}
