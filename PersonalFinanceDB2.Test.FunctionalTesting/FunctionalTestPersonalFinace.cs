using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PersonalFinanceDB2;
using PersonalFinanceDB2.Data;

[TestFixture]
public class FunctionalTests
{
    private DbContextOptions<PersonalFinanceDbContext> _dbContextOptions;

    [SetUp]
    public void Setup()
    {
        _dbContextOptions = new DbContextOptionsBuilder<PersonalFinanceDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
    }
    [Test]
    public void AddFullReceipt_Should_Add_Receipt_And_PurchaseDetails()
    {
        using (var dbContext = new PersonalFinanceDbContext(_dbContextOptions))
        {
            var tempReceipt = new TempReceipt
            {
                StoreName = "Test Store",
                DateTime = "2020-12-30",
                TotalAmount = "15.50"
            };

            var tempDetails = new List<TempDetails>
            {
                new TempDetails { ProductName = "Milk", Quantity = "2", Amount = "10", Category = "Groceries", Subcategory = "Milk" },
                new TempDetails { ProductName = "Bread", Quantity = "1", Amount = "5.50", Category = "Groceries", Subcategory = "Bread" }
            };
            FinanceDb.AddFullReceipt(dbContext, tempReceipt, tempDetails);

            var receipt = dbContext.Receipts.Include(r => r.Store).FirstOrDefault();
            Assert.That("Test Store", Is.EqualTo(receipt.Store.Name));
            Assert.That(Convert.ToDateTime("2020-12-30"), Is.EqualTo(receipt.DateTime));
            Assert.That(15.50m, Is.EqualTo(receipt.TotalAmount));

            var purchaseDetails = dbContext.PurchaseDetails.ToList();
            Assert.That(2, Is.EqualTo(purchaseDetails.Count));

            var products = dbContext.Products.ToList();
            Assert.That(products[0].Name, Is.EqualTo("Milk"));
            Assert.That(products[1].Name, Is.EqualTo("Bread"));
            Assert.That(products[0].Subcategory.Name, Is.EqualTo("Milk"));
            Assert.That(products[1].Subcategory.Name, Is.EqualTo("Bread"));
            Assert.That(products[0].Category.Name, Is.EqualTo("Groceries"));
        }
    }
}