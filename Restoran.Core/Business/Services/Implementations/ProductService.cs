using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restoran.Core.Business.Services.Interfaces;
using Restoran.Core.DTOs.Product;
using Restoran.Core.Statics;
using Restoran.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restoran.Core.Business.Services.Implementations
{
    public class ProductService : IProductService // Arayüzü uyguluyor
    {
        private readonly IMapper _mapper;

        // Constructor: Sadece IMapper'ı alıyor, bağlantı dizesini AppConfiguration'dan çekecek
        public ProductService(IMapper mapper)
        {
            _mapper = mapper;
        }

        // DbContext'i her işlem için doğrudan ve bağımsız olarak oluşturan yardımcı metot
        private RestaurantDbContext CreateDbContext()
        {
            // Bağlantı dizesinin uygulama başlangıcında ayarlandığından emin oluyoruz
            if (string.IsNullOrEmpty(AppConfiguration.DefaultConnectionString))
            {
                throw new InvalidOperationException("Veritabanı bağlantı dizesi yapılandırılmadı. Lütfen AppConfiguration.DefaultConnectionString'in uygulama başlangıcında ayarlandığından emin olun.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<RestaurantDbContext>();

            // Veritabanı sağlayıcısını ve bağlantı dizesini ayarlıyoruz
            optionsBuilder.UseSqlServer(AppConfiguration.DefaultConnectionString);
            // Eğer PostgreSQL kullanıyorsanız yukarıdaki satırı yorum satırı yapın ve aşağıdaki satırı aktif edin:
            // optionsBuilder.UseNpgsql(AppConfiguration.DefaultConnectionString); 

            return new RestaurantDbContext(optionsBuilder.Options);
        }

        public async Task<List<ProductListDto>> GetProductsAsync()
        {
            // Veritabanı bağlantısı 'using' bloğu içinde kurulur ve iş bitince serbest bırakılır
            using (var context = CreateDbContext())
            {
                var products = await context.Products
                                            // Category bilgisini de dahil et, DTO için gerekli
                                            .Include(p => p.Category)
                                            .ToListAsync();

                // Entity'leri DTO'lara dönüştür ve geri dön
                return _mapper.Map<List<ProductListDto>>(products);
            }
        }
    }
}
