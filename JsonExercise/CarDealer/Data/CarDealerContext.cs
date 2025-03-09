using Microsoft.EntityFrameworkCore;
using CarDealer.Models;

namespace CarDealer.Data
{
    public class CarDealerContext : DbContext
    {
        public CarDealerContext()
        {
        }

        public CarDealerContext(DbContextOptions options)
            : base(options)
        {
        }

        public virtual DbSet<Car> Cars { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Part> Parts { get; set; }
        public virtual DbSet<PartCar> PartsCars { get; set; }
        public virtual DbSet<Sale> Sales { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
			modelBuilder.Entity<PartCar>()
				.HasKey(pc => new { pc.PartId, pc.CarId });

			modelBuilder.Entity<PartCar>()
				.HasOne(pc => pc.Part)
				.WithMany(p => p.PartsCars)
				.HasForeignKey(pc => pc.PartId);

			modelBuilder.Entity<PartCar>()
				.HasOne(pc => pc.Car)
				.WithMany(c => c.PartsCars)
				.HasForeignKey(pc => pc.CarId);

			modelBuilder.Entity<Part>()
				.HasOne(p => p.Supplier)
				.WithMany(s => s.Parts)
				.HasForeignKey(p => p.SupplierId);

			modelBuilder.Entity<Sale>()
				.HasOne(s => s.Car)
				.WithMany(c => c.Sales)
				.HasForeignKey(s => s.CarId);

			modelBuilder.Entity<Sale>()
				.HasOne(s => s.Customer)
				.WithMany(c => c.Sales)
				.HasForeignKey(s => s.CustomerId);

			modelBuilder.Entity<Part>()
				.Property(p => p.Price)
				.HasPrecision(18, 2);

			modelBuilder.Entity<Sale>()
				.Property(s => s.Discount)
				.HasPrecision(18, 2);
		}
    }
}
