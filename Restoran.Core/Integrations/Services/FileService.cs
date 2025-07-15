using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restoran.Core.Integrations.Services
{
    public static class FileService
    {
        private static readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };
        private static readonly long _maxFileSize = 5 * 1024 * 1024; 
        private static readonly string _uploadPath = "uploads";

        public static (bool Success, string Message, string? FileName) SaveFile(byte[] fileData, string fileName, string webRootPath = "")
        {
            try
            {
                // Dosya uzantısını kontrol et
                var extension = Path.GetExtension(fileName).ToLower();
                if (!_allowedExtensions.Contains(extension))
                {
                    return (false, "Sadece jpg, jpeg, png dosyaları kabul edilir.", null);
                }

                // Dosya boyutunu kontrol et
                if (fileData.Length > _maxFileSize)
                {
                    return (false, "Dosya boyutu 5MB'dan büyük olamaz.", null);
                }

                // Tam path oluştur
                var fullUploadPath = string.IsNullOrEmpty(webRootPath) 
                    ? _uploadPath 
                    : Path.Combine(webRootPath, _uploadPath);

                // Upload klasörünü oluştur
                if (!Directory.Exists(fullUploadPath))
                {
                    Directory.CreateDirectory(fullUploadPath);
                }

                // Benzersiz dosya adı oluştur
                var newFileName = $"{Guid.NewGuid()}{extension}";
                var fullPath = Path.Combine(fullUploadPath, newFileName);

                // Dosyayı kaydet
                File.WriteAllBytes(fullPath, fileData);

                return (true, "Dosya başarıyla yüklendi.", newFileName);
            }
            catch (Exception ex)
            {
                return (false, $"Dosya yükleme hatası: {ex.Message}", null);
            }
        }

        public static (bool Success, string Message) DeleteFile(string fileName, string webRootPath = "")
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    return (true, "Silinecek dosya bulunamadı.");
                }

                var fullUploadPath = string.IsNullOrEmpty(webRootPath) 
                    ? _uploadPath 
                    : Path.Combine(webRootPath, _uploadPath);

                var fullPath = Path.Combine(fullUploadPath, fileName);
                
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return (true, "Dosya başarıyla silindi.");
                }

                return (true, "Dosya zaten mevcut değil.");
            }
            catch (Exception ex)
            {
                return (false, $"Dosya silme hatası: {ex.Message}");
            }
        }

        public static string GetFileUrl(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return string.Empty;

            return $"/{_uploadPath}/{fileName}";
        }
    }
} 