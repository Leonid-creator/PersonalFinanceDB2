using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceDB2.Data;

namespace PersonalFinanceDB2
{
    internal class FinanceRepository
    {
        private readonly PersonalFinanceDbContext DbContext;
        public FinanceRepository(PersonalFinanceDbContext context)
        {
            DbContext = context;
        }
        public void AddFullReceipt(TempReceipt tempReceipt, List<TempDetails> tempDetails)
        {
            int receiptID = AddBriefReceiptInfo(tempReceipt);
            for (int i = 0; i < tempDetails.Count; i++)
            {
                AddPurchaseDetail(tempDetails[i], receiptID);
            }
            DbContext.SaveChanges();
        }
        private int AddBriefReceiptInfo(TempReceipt tempReceipt)
        {
            Store store = DbContext.Stores.FirstOrDefault(s => s.Name == tempReceipt.StoreName);
            if (store == null)
            {
                store = AddStore(tempReceipt.StoreName);
            }

            Receipt newReceipt = new Receipt()
            {
                StoreID = store.StoreID,
                DateTime = Convert.ToDateTime(tempReceipt.DateTime),
                TotalAmount = decimal.Parse(tempReceipt.TotalAmount)
            };
            var existingReceipt = DbContext.Receipts.FirstOrDefault(r => r.StoreID == store.StoreID
                                                                    && r.DateTime == newReceipt.DateTime
                                                                    && r.TotalAmount == newReceipt.TotalAmount);

            if (existingReceipt == null)
            {
                DbContext.Receipts.Add(newReceipt);
                DbContext.SaveChanges();
            }
            else
            {
                throw new DuplicateNameException($"Receipt {tempReceipt.StoreName} {tempReceipt.DateTime} already exists");
            }
            return newReceipt.ReceiptID;
        }
        private void AddPurchaseDetail(TempDetails tempDetails, int receiptID)
        {
            Product product = DbContext.Products.FirstOrDefault(s => s.Name == tempDetails.ProductName);
            if (product == null)
            {
                product = AddProduct(tempDetails);
            }

            PurchaseDetail newPurchaseDetail = new PurchaseDetail
            {
                ReceiptID = receiptID,
                ProductID = product.ProductID,
                Quantity = int.Parse(tempDetails.Quantity),
                Amount = decimal.Parse(tempDetails.Amount)
            };
            DbContext.PurchaseDetails.Add(newPurchaseDetail);
        }
        private Store AddStore(string storeName)
        {
            Store newStore = new Store { Name = storeName };
            DbContext.Stores.Add(newStore);
            DbContext.SaveChanges();
            return newStore;
        }
        private Product AddProduct(TempDetails tempDetails)
        {
            Subcategory subcategory = DbContext.Subcategories.FirstOrDefault(s => s.Name == tempDetails.Subcategory);
            if (subcategory == null)
            {
                subcategory = AddSubcategory(tempDetails.Subcategory, tempDetails.Category);
            }

            Product newProduct = new Product
            {
                Name = tempDetails.ProductName,
                SubcategoryID = subcategory.SubcategoryID,
                CategoryID = subcategory.CategoryID
            };
            DbContext.Products.Add(newProduct);
            DbContext.SaveChanges();
            return newProduct;
        }
        public Subcategory AddSubcategory(string subcategory, string category)
        {
            Category newCategory = DbContext.Categories.FirstOrDefault(c => c.Name == category);
            if (newCategory == null)
            {
                newCategory = AddCategory(category);
            }

            Subcategory newSubcategory = new Subcategory
            {
                Name = subcategory,
                CategoryID = newCategory.CategoryID
            };
            DbContext.Subcategories.Add(newSubcategory);
            DbContext.SaveChanges();
            return newSubcategory;
        }
        public Category AddCategory(string category)
        {
            Category newCategory = new Category { Name = category };
            DbContext.Categories.Add(newCategory);
            DbContext.SaveChanges();
            return newCategory;
        }
    }
}
