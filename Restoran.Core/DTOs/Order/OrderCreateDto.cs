using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restoran.Core.DTOs.Order
{
    public class OrderCreateDto
    {
        public int UserId { get; set; } // Siparişi veren kullanıcının Id'si
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }
}
