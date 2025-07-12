using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restoran.Core.DTOs.User
{
    public class UserRegisterDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // Şifre hashlenmeden önce
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        // UserRole'u direkt register sırasında belirlemek istemeyebiliriz, varsayılan Customer olabilir
        // Ya da adminin belirlemesi gereken bir durumsa burada olmaz. Şimdilik yok.
    }
}
