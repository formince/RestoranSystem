using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restoran.Core.Entity
{
    public class Category : BaseEntity 
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(300)]
        public string Description { get; set; } = string.Empty;
        
        public int DisplayOrder { get; set; }
        
        [StringLength(255)]
        public string ImageUrl { get; set; } = string.Empty;

        public ICollection<Product>? Products { get; set; }
    }
}
