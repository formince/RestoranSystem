using Restoran.Core.Statics.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restoran.Core.DTOs.User
{
    public class UserDetailDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        // Siparişleri ve rezervasyonları detayda göstermek isterseniz buraya ekleyebilirsiniz
        // public List<OrderListDto> Orders { get; set; } = new List<OrderListDto>();
        // public List<ReservationListDto> Reservations { get; set; } = new List<ReservationListDto>();
    }
}
