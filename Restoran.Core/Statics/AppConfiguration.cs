using Microsoft.EntityFrameworkCore;
using Restoran.Core.Data;
using System;

namespace Restoran.Core.Statics
{
    public static class AppConfiguration 
    {     
        private static string? _defaultConnectionString;
        
        public static string DefaultConnectionString 
        { 
            get 
            {
                if (string.IsNullOrEmpty(_defaultConnectionString))
                {
                    throw new InvalidOperationException("Connection string ayarlanmamış. Lütfen AppConfiguration.SetConnectionString() methodunu çağırın.");
                }
                return _defaultConnectionString;
            }
            private set 
            {
                _defaultConnectionString = value;
            }
        }
        
        /// <summary>
        /// Connection string'i ayarlar. Uygulama başlangıcında bir kez çağrılmalıdır.
        /// </summary>
        public static void SetConnectionString(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Connection string boş olamaz.", nameof(connectionString));
            }
            DefaultConnectionString = connectionString;
        }
        
        /// <summary>
        /// Yeni bir DbContext instance'ı oluşturur. Using pattern ile kullanılmalıdır.
        /// </summary>
        public static RestaurantDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<RestaurantDbContext>();
            optionsBuilder.UseSqlServer(DefaultConnectionString);
            return new RestaurantDbContext(optionsBuilder.Options);
        }
        
        /// <summary>
        /// Connection string'in ayarlanıp ayarlanmadığını kontrol eder.
        /// </summary>
        public static bool IsConfigured => !string.IsNullOrEmpty(_defaultConnectionString);
    }
}
