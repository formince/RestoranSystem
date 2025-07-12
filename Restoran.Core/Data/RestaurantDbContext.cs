using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Restoran.Core.Entity;

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
        }

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