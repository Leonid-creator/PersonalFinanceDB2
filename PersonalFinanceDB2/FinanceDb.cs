using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;

namespace PersonalFinanceDB2
{
    public class FinanceDb
    {
        public static void AddFullReceipt(string filePath)
        {
            using (var dbContext = new PersonalFinanceDbContext())
            {
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        bool isReadingReceiptInfo = true;
                        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                        {
                            IgnoreBlankLines = false
                        };

                        var reader = new StreamReader(filePath);
                        var csv = new CsvReader(reader, config);
                        Receipt newReceipt = new Receipt();

                        csv.Read();
                        while (csv.Read())
                        {
                            string[] fields = csv.Parser.Record;
                            if (string.IsNullOrWhiteSpace(fields[0]) || fields[0] == null)
                            {
                                isReadingReceiptInfo = true;
                                continue;
                            }
                            for (int i = 0; i < fields.Length; i++)
                            {
                                fields[i] = fields[i].Trim();
                            }

                            if (isReadingReceiptInfo)
                            {
                                isReadingReceiptInfo = false;
                                newReceipt = FinanceDb.AddBriefReceipt(dbContext, fields);
                                Console.WriteLine($"new receipt ({fields[0]})");          //just for test, delete it
                            }
                            else
                            {
                                FinanceDb.AddPurchaseDetails(dbContext, fields, newReceipt.ReceiptID);
                                Console.WriteLine($"new detail ({fields[0]})");          //just for test, delete it
                            }
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("transaction.Rollback");          //just for test, delete it
                        Console.WriteLine(ex);
                        transaction.Rollback();
                    }
                }
            }
        }
        public static Receipt AddBriefReceipt(PersonalFinanceDbContext dbContext, string[] fields)
        {
            string storeName = fields[0];
            DateTime dateTime = Convert.ToDateTime(fields[1]);
            decimal totalAmount = decimal.Parse(fields[2]);

            Store store = dbContext.Stores.FirstOrDefault(s => s.Name == storeName);
            if (store == null)
            {
                store = AddStore(dbContext, storeName);
            }

            Receipt newReceipt = new Receipt()
            {
                StoreID = store.StoreID,
                DateTime = dateTime,
                TotalAmount = totalAmount
            };
            var existingReceipt = dbContext.Receipts.FirstOrDefault(r => r.StoreID == store.StoreID
                                                                    && r.DateTime == dateTime
                                                                    && r.TotalAmount == totalAmount);

            if(existingReceipt == null)
            {
                dbContext.Receipts.Add(newReceipt);
                dbContext.SaveChanges();
            }
            else
            {
                Console.WriteLine("This receipt already exist");
            }
            return newReceipt;
        }

        public static void AddPurchaseDetails(PersonalFinanceDbContext dbContext, string[] fields, int receiptID)
        {
            string productName = fields[0];
            int quantity = int.Parse(fields[1]);
            decimal amount = decimal.Parse(fields[2]);
            
            Product product = dbContext.Products.FirstOrDefault(s => s.Name == productName);
            if (product == null)
            {
                product = AddProduct(dbContext, productName);
            }

            PurchaseDetail newPurchaseDetail = new PurchaseDetail
            {
                ReceiptID = receiptID,
                ProductID = product.ProductID,
                Quantity = quantity,
                Amount = amount
            };
            dbContext.PurchaseDetails.Add(newPurchaseDetail);
            dbContext.SaveChanges();
        }

        public static Store AddStore(PersonalFinanceDbContext dbContext, string storeName)
        {
            Store newStore = new Store { Name = storeName };
            dbContext.Stores.Add(newStore);
            dbContext.SaveChanges();
            return newStore;
        }
        public static Product AddProduct(PersonalFinanceDbContext dbContext, string productName)
        {
            Product newProduct = new Product { Name = productName };
            dbContext.Products.Add(newProduct);
            dbContext.SaveChanges();
            return newProduct;
        }
    }
}
