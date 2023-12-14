using System;
using System.ComponentModel.DataAnnotations;

namespace DF_EvolutionAPI.Models
{
    public class BaseEntity
    {
        [Required]
        public int Id { get; set; }

        public int? CreateBy { get; set; }

        public int? UpdateBy { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

    }
}
