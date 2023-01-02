using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace AjcProject.Models
{
    public partial class AjcProjectDbContext : DbContext
    {
        public AjcProjectDbContext()
        {
        }

        public AjcProjectDbContext(DbContextOptions<AjcProjectDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=SqlDbConnection");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => e.ItemId)
                    .HasName("PK_dbo.CartItems");

                entity.HasIndex(e => e.ProductId, "IX_ProductId");

                entity.Property(e => e.ItemId).HasMaxLength(128);

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_dbo.CartItems_dbo.Products_ProductId");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            //modelBuilder.Entity<Order>(entity =>
            //{
            //    entity.Property(e => e.Address)
            //        .IsRequired()
            //        .HasMaxLength(70);

            //    entity.Property(e => e.City)
            //        .IsRequired()
            //        .HasMaxLength(40);

            //    entity.Property(e => e.Country)
            //        .IsRequired()
            //        .HasMaxLength(40);

            //    entity.Property(e => e.Email).IsRequired();

            //    entity.Property(e => e.FirstName)
            //        .IsRequired()
            //        .HasMaxLength(160);

            //    entity.Property(e => e.LastName)
            //        .IsRequired()
            //        .HasMaxLength(160);

            //    entity.Property(e => e.OrderDate).HasColumnType("datetime");

            //    entity.Property(e => e.Phone).HasMaxLength(24);

            //    entity.Property(e => e.PostalCode)
            //        .IsRequired()
            //        .HasMaxLength(10);

            //    entity.Property(e => e.State)
            //        .IsRequired()
            //        .HasMaxLength(40);

            //    entity.Property(e => e.Total).HasColumnType("decimal(18, 2)");
            //});

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasIndex(e => e.OrderId, "IX_OrderId");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_dbo.OrderDetails_dbo.Orders_OrderId");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.CategoryId, "IX_CategoryID");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_dbo.Products_dbo.Categories_CategoryID");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
