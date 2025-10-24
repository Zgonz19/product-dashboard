using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Xml.Linq;
using ProductAPI.Data.Models;

namespace ProductAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Description).HasMaxLength(500);
                entity.Property(c => c.IsActive).IsRequired();

                entity.HasIndex(c => c.IsActive);
                entity.HasIndex(c => c.Name);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Description).HasMaxLength(1000);
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
                entity.Property(p => p.StockQuantity).IsRequired();
                entity.Property(p => p.CreatedDate).IsRequired();
                entity.Property(p => p.IsActive).IsRequired();

                entity.HasOne(p => p.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.CategoryId);

                entity.HasIndex(p => p.CategoryId);
                entity.HasIndex(p => p.Name);
                entity.HasIndex(p => p.Price);
                entity.HasIndex(p => p.StockQuantity);
                entity.HasIndex(p => p.IsActive);

                //CREATE FULLTEXT INDEX ON Products(Name, Description)
                //KEY INDEX PK_Products;
            });


        }
    }

}
