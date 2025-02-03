using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using CsvHelper;
using CsvHelper.Configuration;

namespace PersonalFinanceDB2
{
    public class FinanceDb
    {
        public static void AddReceiptByConsole()
        {
            TempReceipt tempReceipt = new TempReceipt();
            List<TempDetails> tempDetails = new List<TempDetails>();

            tempReceipt = EnterTempReceipt();
            while (true)
            {
                tempDetails.Add(EnterTempDetails());
                Console.WriteLine("New product?");
                if (Console.ReadLine() == "n")
                {
                    break;
                }
            }
            while (true)
            {
                Console.WriteLine("Check data:");
                ShowReceiptData(tempReceipt, tempDetails);
                Console.WriteLine("Save data?");
                Console.WriteLine("y/n or e (edit)");
                string action = Console.ReadLine();
                if (action == "y")
                {
                    ProcessReceipt(tempReceipt, tempDetails);
                    break;
                }else if (action == "n")
                {
                    Console.WriteLine("Adding check interrupted");
                    break;
                }
                else if (action == "e")
                {
                    Console.WriteLine("which line?");
                    int line = int.Parse(Console.ReadLine());
                    if (line == 1)
                    {
                        tempReceipt = EnterTempReceipt();
                    }
                    else
                    {
                        tempDetails[line - 2] = EnterTempDetails();
                    }
                }
            }
        }
        public static void AddReceiptByCSV(string filePath)
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
                        int receiptID = 0;

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
                                TempReceipt tempReceipt = new TempReceipt
                                {
                                    StoreName = fields[0],
                                    DateTime = fields[1],
                                    TotalAmount = fields[2]
                                };
                                receiptID = AddBriefReceiptInfo(dbContext, tempReceipt);
                                Console.WriteLine($"new receipt ({fields[0]})");          //just for test, delete it
                            }
                            else
                            {
                                TempDetails tempDetails = new TempDetails
                                {
                                    ProductName = fields[0],
                                    Quantity = fields[1],
                                    Amount = fields[2]
                                };
                                AddPurchaseDetail(dbContext, tempDetails, receiptID);
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
        private static TempReceipt EnterTempReceipt()
        {
            TempReceipt tempReceipt = new TempReceipt();
            tempReceipt.StoreName = GetInput("Store name:");
            tempReceipt.DateTime = GetInput("Date and time:");
            tempReceipt.TotalAmount = GetInput("Total amount:");
            return tempReceipt;
        }
        private static TempDetails EnterTempDetails()
        {
            TempDetails tempDetails = new TempDetails();
            tempDetails.ProductName = GetInput("Product name:");
            tempDetails.Quantity = GetInput("Quantity:");
            tempDetails.Amount = GetInput("Amount:");
            return tempDetails;
        }
        private static string GetInput(string message)
        {
            Console.WriteLine(message);
            return Console.ReadLine();
        }
        private static void ShowReceiptData(TempReceipt tempReceipt, List<TempDetails> tempDetails)
        {
            Console.WriteLine($"\tStore name: {tempReceipt.StoreName}");
            Console.WriteLine($"\tDate and time: {tempReceipt.DateTime}");
            Console.WriteLine($"\tTotal amount: {tempReceipt.TotalAmount}");
            foreach (var item in tempDetails)
            {
                Console.WriteLine($"\tProduct name: {item.ProductName} | Quantity: {item.Quantity} | Amount: {item.Amount}");
            }
        }
        public static void ProcessReceipt(TempReceipt tempReceipt, List<TempDetails> tempDetails)
        {
            using (PersonalFinanceDbContext dbContext = new PersonalFinanceDbContext())
            {
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        AddFullReceipt(dbContext, tempReceipt, tempDetails);
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
        public static void AddFullReceipt(PersonalFinanceDbContext dbContext, TempReceipt tempReceipt, List<TempDetails> tempDetails)
        {
            int receiptID = AddBriefReceiptInfo(dbContext, tempReceipt);
            for (int i = 0; i < tempDetails.Count; i++)
            {
                AddPurchaseDetail(dbContext, tempDetails[i], receiptID);
            }
            dbContext.SaveChanges();
        }
        private static int AddBriefReceiptInfo(PersonalFinanceDbContext dbContext, TempReceipt tempReceipt)
        {
            Store store = dbContext.Stores.FirstOrDefault(s => s.Name == tempReceipt.StoreName);
            if (store == null)
            {
                store = AddStore(dbContext, tempReceipt.StoreName);
            }

            Receipt newReceipt = new Receipt()
            {
                StoreID = store.StoreID,
                DateTime = Convert.ToDateTime(tempReceipt.DateTime),
                TotalAmount = decimal.Parse(tempReceipt.TotalAmount)
            };
            var existingReceipt = dbContext.Receipts.FirstOrDefault(r => r.StoreID == store.StoreID
                                                                    && r.DateTime == newReceipt.DateTime
                                                                    && r.TotalAmount == newReceipt.TotalAmount);

            if(existingReceipt == null)
            {
                dbContext.Receipts.Add(newReceipt);
                dbContext.SaveChanges();
            }
            else
            {
                Console.WriteLine("This receipt already exist");
            }
            return newReceipt.ReceiptID;
        }
        private static void AddPurchaseDetail(PersonalFinanceDbContext dbContext, TempDetails tempDetails, int receiptID)
        {
            Product product = dbContext.Products.FirstOrDefault(s => s.Name == tempDetails.ProductName);
            if (product == null)
            {
                product = AddProduct(dbContext, tempDetails.ProductName);
            }

            PurchaseDetail newPurchaseDetail = new PurchaseDetail
            {
                ReceiptID = receiptID,
                ProductID = product.ProductID,
                Quantity = int.Parse(tempDetails.Quantity),
                Amount = decimal.Parse(tempDetails.Amount)
            };
            dbContext.PurchaseDetails.Add(newPurchaseDetail);
            dbContext.SaveChanges();
        }
        private static Store AddStore(PersonalFinanceDbContext dbContext, string storeName)
        {
            Store newStore = new Store { Name = storeName };
            dbContext.Stores.Add(newStore);
            dbContext.SaveChanges();
            return newStore;
        }
        private static Product AddProduct(PersonalFinanceDbContext dbContext, string productName)
        {
            Product newProduct = new Product { Name = productName };
            dbContext.Products.Add(newProduct);
            dbContext.SaveChanges();
            return newProduct;
        }
    }
}
