using Microsoft.EntityFrameworkCore;
using SewingAPI.Domain.Entities;

namespace SewingAPI.Repo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Contragent> Contragents => Set<Contragent>();
        public DbSet<Material> Materials => Set<Material>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<WarehouseItem> WarehouseItems => Set<WarehouseItem>();
        public DbSet<Fill> Fills => Set<Fill>();
        public DbSet<AppUser> Users => Set<AppUser>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employees");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Fio).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Position).HasMaxLength(120).IsRequired();
            });

            modelBuilder.Entity<Contragent>(entity =>
            {
                entity.ToTable("Contragents");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Contact).HasMaxLength(200);
                entity.Property(e => e.Phone).HasMaxLength(50);
            });

            modelBuilder.Entity<Material>(entity =>
            {
                entity.ToTable("Materials");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Color).HasMaxLength(100);
                entity.Property(e => e.Article).HasMaxLength(100);
                entity.Property(e => e.Unit).HasMaxLength(30).IsRequired();
                entity.Property(e => e.Qty).HasPrecision(18, 3);
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.Property(e => e.Sum).HasPrecision(18, 2);

                entity.HasOne(e => e.Contragent)
                    .WithMany(e => e.Materials)
                    .HasForeignKey(e => e.ContragentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Num).HasMaxLength(80);
                entity.Property(e => e.Total).HasPrecision(18, 2);

                entity.HasOne(e => e.Contragent)
                    .WithMany(e => e.Orders)
                    .HasForeignKey(e => e.ContragentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<WarehouseItem>(entity =>
            {
                entity.ToTable("WarehouseItems");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Qty).HasPrecision(18, 3);
                entity.Property(e => e.Price).HasPrecision(18, 2);

                entity.HasIndex(e => e.MaterialId).IsUnique();

                entity.HasOne(e => e.Material)
                    .WithOne(e => e.WarehouseItem)
                    .HasForeignKey<WarehouseItem>(e => e.MaterialId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Contragent)
                    .WithMany()
                    .HasForeignKey(e => e.ContragentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Fill>(entity =>
            {
                entity.ToTable("Fills");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Type).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500).IsRequired();
                entity.Property(e => e.Related).HasMaxLength(150).IsRequired();
                entity.Property(e => e.Status).HasMaxLength(60).IsRequired();
            });

            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.Username).HasMaxLength(120).IsRequired();
                entity.Property(e => e.PasswordHash).HasMaxLength(300).IsRequired();
                entity.Property(e => e.Role).HasMaxLength(50).IsRequired();
                entity.Property(e => e.RefreshToken).HasMaxLength(200);
            });
        }
    }
}
