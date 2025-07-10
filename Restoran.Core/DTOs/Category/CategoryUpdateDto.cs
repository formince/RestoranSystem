using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restoran.Core.DTOs.Category
{
    public class CategoryUpdateDto
    {
        public int Id { get; set; } // Update için Id gerekli
        public string Name { get; set; } = string.Empty;
    }
}
