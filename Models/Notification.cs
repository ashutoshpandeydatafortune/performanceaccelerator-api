using System;
using System.ComponentModel.DataAnnotations;

namespace DF_EvolutionAPI.Models
{
    public class Notification
    {
        [Key]
        public int? Id { get; set; }
        public int? ResourceId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public byte? IsRead { get; set; }
        public byte? IsActive { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
