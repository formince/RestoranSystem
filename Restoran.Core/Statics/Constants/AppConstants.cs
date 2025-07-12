using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restoran.Core.Statics.Constants
{
    public static class AppConstants
    {
        // Uygulama Bilgileri
        public const string ApplicationName = "Restoran Sistemi";
        public const string Version = "1.0.0";
        public const string Company = "Restoran Yönetim Sistemi";
        public const string Developer = "Yunus Emre";
        public const string Copyright = "© 2024 Restoran Sistemi. Tüm hakları saklıdır.";

        // Dosya Yükleme
        public const long MaxFileSize = 5 * 1024 * 1024; // 5MB
        public static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png" };

        // İş Kuralları
        public const decimal MinOrderAmount = 50.00m;
        public const int WorkingHourStart = 9;
        public const int WorkingHourEnd = 22;
        public const int MaxReservationDaysInAdvance = 30;
        public const int MaxTableCapacity = 20;
        public const int MinTableCapacity = 1;
        public const int MinPasswordLength = 6;

        // Sistem Ayarları
        public const int DefaultPageSize = 10;
        public const int SessionTimeoutMinutes = 30;
        
        // Mesajlar
        public const string SuccessMessage = "İşlem başarıyla tamamlandı.";
        public const string ErrorMessage = "Bir hata oluştu. Lütfen tekrar deneyin.";
        public const string NotFoundMessage = "Kayıt bulunamadı.";
        public const string UnauthorizedMessage = "Bu işlem için yetkiniz yok.";
    }
} 