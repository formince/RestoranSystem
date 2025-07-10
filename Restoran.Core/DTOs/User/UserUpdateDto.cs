using Restoran.Core.Statics.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restoran.Core.DTOs.User
{
    public class UserUpdateDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; } // Rol de güncellenebilir (Admin paneli için)
        // Şifre güncellemeyi ayrı bir DTO veya metotla yapmak daha güvenlidir.
    }
}
