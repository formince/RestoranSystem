using Restoran.Core.Statics.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restoran.Core.Entity
{
    public class Reservation : BaseEntity 
    {
        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(15)]
        public string CustomerPhone { get; set; } = string.Empty;
        
        [EmailAddress]
        [StringLength(100)]
        public string CustomerEmail { get; set; } = string.Empty;
        
        [Required]
        public DateTime ReservationDateTime { get; set; }
        
        [Required]
        [Range(1, 20)]
        public int NumberOfGuests { get; set; }
        
        [StringLength(300)]
        public string SpecialRequests { get; set; } = string.Empty;

        [Required]
        public int UserId { get; set; }
        
        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        public int TableId { get; set; }
        
        [ForeignKey("TableId")]
        public Table? Table { get; set; }

        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    }
}
