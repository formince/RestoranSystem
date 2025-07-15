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
    public class BLLUser 
    {
        public BLLUser()
        {
           
        }

        private RestaurantDbContext CreateContext()
        {
            return new RestoranDbContextFactory().CreateDbContext();
        }

        public async Task<List<UserListDto>> GetUsersAsync()
        {
            using var context = CreateContext();

            var users = await context.Users.ToListAsync();

            var userListDtos = new List<UserListDto>();
            foreach (var user in users)
            {
                userListDtos.Add(new UserListDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role
                });
            }
            return userListDtos;
        }

        public async Task<UserDetailDto?> GetUserByIdAsync(int id)
        {
            using var context = CreateContext();

            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return null;

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

        public async Task<bool> CreateUserAsync(UserRegisterDto dto)
        {
            using var context = CreateContext();

            
            var authBll = new BLLAuth();
            var registerResult = await authBll.RegisterAsync(dto);
            
            return registerResult.Success;
        }

        public async Task<bool> UpdateUserAsync(int id, UserUpdateDto dto)
        {
            using var context = CreateContext();

            var user = await context.Users.FindAsync(id);
            if (user == null) return false;

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Username = dto.Username;
            user.Email = dto.Email;
            user.Phone = dto.Phone;
            user.Role = dto.Role;

            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            using var context = CreateContext();

            var user = await context.Users.FindAsync(id);
            if (user == null) return false;

            user.IsActive = false;

            return await context.SaveChangesAsync() > 0;
        }

        
        public async Task<(bool Success, string Message)> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            using var context = CreateContext();

            var user = await context.Users.FindAsync(userId);
            if (user == null) return (false, "Kullanıcı bulunamadı");

            var authBll = new BLLAuth();
            
           
            var loginDto = new UserLoginDto { Username = user.Username, Password = oldPassword };
            var loginResult = await authBll.LoginAsync(loginDto);
            if (loginResult == null) return (false, "Mevcut şifre yanlış");

           
            if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 6)
                return (false, "Yeni şifre en az 6 karakter olmalıdır");

            
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            var success = await context.SaveChangesAsync() > 0;
            
            return success ? (true, "Şifre başarıyla güncellendi") : (false, "Şifre güncellenemedi");
        }


    }
} 