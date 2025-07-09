using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restoran.Core.Entity
{
    public class Table : BaseEntity 
    {
        [Required]
        [StringLength(10)]
        public string TableNumber { get; set; } = string.Empty;
        
        [Required]
        [Range(1, 20)]
        public int Capacity { get; set; }
        
        public bool IsAvailable { get; set; } = true;
        
        [StringLength(100)]
        public string Location { get; set; } = string.Empty; // örn: "Balkon", "Ana Salon", "Bahçe"
        
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        public ICollection<Reservation>? Reservations { get; set; }
    }
}
