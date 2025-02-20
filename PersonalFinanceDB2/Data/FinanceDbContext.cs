using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinanceDB2.Data
{
    public class PersonalFinanceDbContext : DbContext
    {
        private readonly string _connectionString;
        public PersonalFinanceDbContext() { }
        public PersonalFinanceDbContext(DbContextOptions<PersonalFinanceDbContext> options) : base(options) { }
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<PurchaseDetail> PurchaseDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Subcategory> Subcategories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;" +
                "AttachDbFileName=C:\\Users\\lenin\\source\\repos\\Leonid-creator\\PersonalFinanceDB2\\PersonalFinanceDB2\\PersonalFinanceDB2\\PersonalFinanceDB2.mdf;" +
                "Database=personalFinanceDB2;Trusted_Connection=True;");
            }
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
                .HasForeignKey(r => r.StoreID)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Receipt>()
                .Property(r => r.Comment)
                .IsRequired(false);

            //PurchaseDetail
            modelBuilder.Entity<PurchaseDetail>()
                .HasOne(pd => pd.Receipt)
                .WithMany(r => r.PurchaseDetails)
                .HasForeignKey(pd => pd.ReceiptID)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<PurchaseDetail>()
                .HasOne(pd => pd.Product)
                .WithMany(p => p.PurchaseDetails)
                .HasForeignKey(pd => pd.ProductID)
                .OnDelete(DeleteBehavior.NoAction);

            //Product
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryID)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Subcategory)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SubcategoryID)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Product>()
                .Property(p => p.CategoryID)
                .IsRequired(false);

            //Subcategory
            modelBuilder.Entity<Subcategory>()
                .HasOne(s => s.Category)
                .WithMany(c => c.Subcategories)
                .HasForeignKey(s => s.CategoryID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
