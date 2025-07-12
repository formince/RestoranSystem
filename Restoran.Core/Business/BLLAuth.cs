using Microsoft.EntityFrameworkCore;
using Restoran.Core.Data;
using Restoran.Core.DTOs.User;
using Restoran.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restoran.Core.Business
{
    public class BLLAuth // Sınıf adı BLLAuth olarak değiştirildi
    {
        public BLLAuth()
        {
            // DI ile ilgili hiçbir şey burada olmayacak.
        }

        private RestaurantDbContext CreateContext()
        {
            return new RestoranDbContextFactory().CreateDbContext();
        }

        public async Task<UserDetailDto?> LoginAsync(UserLoginDto dto)
        {
            using var context = CreateContext();

            // Önce kullanıcıyı username ile bul
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Username == dto.Username);

            if (user == null) return null;

            // Şifre doğrula - BCrypt ile
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return null;

            return new UserDetailDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role
            };
        }

        // JWT Token ile giriş - API için
        public async Task<(bool Success, string Message, string? Token)> LoginWithJwtAsync(UserLoginDto dto)
        {
            var userDetail = await LoginAsync(dto);
            if (userDetail == null)
                return (false, "Kullanıcı adı veya şifre hatalı", null);

            var jwtHelper = new BLLJwt();
            var token = jwtHelper.GenerateToken(userDetail);
            
            return (true, "Giriş başarılı", token);
        }

        public async Task<(bool Success, string Message)> RegisterAsync(UserRegisterDto dto)
        {
            using var context = CreateContext();

            // 1. Kullanıcı adı kontrolü
            var existingUser = await context.Users
                .FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (existingUser != null) return (false, "Bu kullanıcı adı zaten kullanılıyor");

            // 2. Email kontrolü
            var existingEmail = await context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existingEmail != null) return (false, "Bu email adresi zaten kullanılıyor");

            // 3. Basit şifre kontrolü
            if (string.IsNullOrEmpty(dto.Password) || dto.Password.Length < 6)
                return (false, "Şifre en az 6 karakter olmalıdır");

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Username = dto.Username,
                Email = dto.Email,
                Phone = dto.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password) // BCrypt ile güvenli hash
            };

            await context.Users.AddAsync(user);
            var success = await context.SaveChangesAsync() > 0;
                        return success ? (true, "Kullanıcı başarıyla kaydedildi") : (false, "Kullanıcı kaydedilemedi");
        }


    }
} 