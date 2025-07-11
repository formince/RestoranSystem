using Restoran.Core.Statics.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restoran.Core.DTOs.Order
{
    public class OrderDetailDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public string? CustomerUsername { get; set; }
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>(); 
    }
}
