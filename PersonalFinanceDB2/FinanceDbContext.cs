using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinanceDB2
{
    public class PersonalFinanceDbContext : DbContext
    {
        public DbSet<Receipt> Receipts{ get; set; }
        public DbSet<Store> Stores{ get; set; }
        public DbSet<PurchaseDetail> PurchaseDetails{ get; set; }
        public DbSet<Product> Products{ get; set; }
        public DbSet<Category> Categories{ get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;" +
                "AttachDbFileName=C:\\Users\\lenin\\source\\repos\\Leonid-creator\\PersonalFinanceDB2\\PersonalFinanceDB2\\PersonalFinanceDB2\\PersonalFinanceDB2.mdf;" +
                "Database=personalFinanceDB2;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Receipt>().HasKey(r => r.ReceiptID);
            modelBuilder.Entity<Store>().HasKey(s => s.StoreID);
            modelBuilder.Entity<PurchaseDetail>().HasKey(pd => new { pd.ReceiptID, pd.ProductID });
            modelBuilder.Entity<Product>().HasKey(p => p.ProductID);
            modelBuilder.Entity<Category>().HasKey(c => c.CategoryID);

            //Receipt
            modelBuilder.Entity<Receipt>()
                .HasOne(r => r.Store)
                .WithMany(s => s.Receipts)
                .HasForeignKey(r => r.StoreID);
            modelBuilder.Entity<Receipt>()
                .Property(r => r.Comment)
                .IsRequired(false);

            //PurchaseDetail
            modelBuilder.Entity<PurchaseDetail>()
                .HasOne(pd => pd.Receipt)
                .WithMany(r => r.PurchaseDetails)
                .HasForeignKey(pd => pd.ReceiptID);
            modelBuilder.Entity<PurchaseDetail>()
                .HasOne(pd => pd.Product)
                .WithMany(p => p.PurchaseDetails)
                .HasForeignKey(pd => pd.ProductID);

            //Product
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryID);
            modelBuilder.Entity<Product>()
                .Property(p => p.CategoryID)
                .IsRequired(false);
        }
    }
}
