using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Restoran.Core.Entity;
using Restoran.Core.Statics.Enums;

namespace Restoran.Core.Data
{

    public class RestoranDbContextFactory : IDesignTimeDbContextFactory<RestaurantDbContext>
    {
        private static string _connectionString=null!;
        private static DbContextOptions<RestaurantDbContext> _options= null!;
        //private static AppDbContext _db;

        public RestaurantDbContext CreateDbContext()
        {
            return CreateDbContext(null!);
        }

        public RestaurantDbContext CreateDbContext(string[] args)
        {
            if (string.IsNullOrEmpty(_connectionString) || _options == null)
                LoadConnectionString();

            return new RestaurantDbContext(_options!);
        }

        private static void LoadConnectionString()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
            //Console.WriteLine(_connectionString);

            // Builder
            var builder = new DbContextOptionsBuilder<RestaurantDbContext>();
            builder.UseSqlServer(_connectionString); // , (opt) => opt.EnableRetryOnFailure(3)
            _options = builder.Options;
        }
    }








    public class RestaurantDbContext : DbContext
    {
        public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Global query filter - Soft delete için
            modelBuilder.Entity<User>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<Category>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<Product>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<Order>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<OrderDetail>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<Table>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<Reservation>().HasQueryFilter(e => e.IsActive);

            // İlişki konfigürasyonları
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Table)
                .WithMany(t => t.Reservations)
                .HasForeignKey(r => r.TableId)
                .OnDelete(DeleteBehavior.Restrict);

            // Decimal property'ler için hassasiyet ayarları
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.UnitPrice)
                .HasColumnType("decimal(18,2)");

            // Seed Data - Admin User
            //SeedData(modelBuilder);
        }

        //private void SeedData(ModelBuilder modelBuilder)
        //{
        //    // Admin User Seed
        //    modelBuilder.Entity<User>().HasData(
        //        new User
        //        {
        //            Id = 1,
        //            FirstName = "Admin",
        //            LastName = "User",
        //            Username = "admin",
        //            Email = "admin@restaurant.com",
        //            Phone = "5551234567",
        //            Role = UserRole.Admin,
        //            PasswordHash = "$2a$11$9YhJ8KHK5LfjQLOZqCpBtu7P8P.eqKhY0IiYKT4d5XqHt0r7RrEVK", // admin123 hash'i
        //            CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        //            IsActive = true
        //        }
        //    );

        //    // Sample Categories
        //    modelBuilder.Entity<Category>().HasData(
        //        new Category
        //        {
        //            Id = 1,
        //            Name = "Ana Yemekler",
        //            Description = "Nefis ana yemek çeşitleri",
        //            DisplayOrder = 1,
        //            ImageUrl = "",
        //            CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        //            IsActive = true
        //        },
        //        new Category
        //        {
        //            Id = 2,
        //            Name = "İçecekler",
        //            Description = "Serinletici içecekler",
        //            DisplayOrder = 2,
        //            ImageUrl = "",
        //            CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        //            IsActive = true
        //        }
        //    );

        //    // Sample Products
        //    modelBuilder.Entity<Product>().HasData(
        //        new Product
        //        {
        //            Id = 1,
        //            Name = "Tavuk Şiş",
        //            Description = "Közde pişmiş tavuk şiş",
        //            Price = 45.00m,
        //            CategoryId = 1,
        //            StockQuantity = 50,
        //            DisplayOrder = 1,
        //            ImageUrl = "",
        //            CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        //            IsActive = true
        //        },
        //        new Product
        //        {
        //            Id = 2,
        //            Name = "Cola",
        //            Description = "Soğuk coca cola",
        //            Price = 8.00m,
        //            CategoryId = 2,
        //            StockQuantity = 100,
        //            DisplayOrder = 1,
        //            ImageUrl = "",
        //            CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        //            IsActive = true
        //        }
        //    );

        //    // Sample Tables
        //    modelBuilder.Entity<Table>().HasData(
        //        new Table
        //        {
        //            Id = 1,
        //            TableNumber = "T001",
        //            Capacity = 4,
        //            IsAvailable = true,
        //            CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        //            IsActive = true
        //        },
        //        new Table
        //        {
        //            Id = 2,
        //            TableNumber = "T002",
        //            Capacity = 6,
        //            IsAvailable = true,
        //            CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        //            IsActive = true
        //        },
        //        new Table
        //        {
        //            Id = 3,
        //            TableNumber = "T003",
        //            Capacity = 2,
        //            IsAvailable = true,
        //            CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        //            IsActive = true
        //        }
        //    );
        //}

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.UtcNow;
                        entry.Entity.IsActive = true;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedDate = DateTime.UtcNow;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
} 