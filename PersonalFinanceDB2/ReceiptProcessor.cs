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
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PersonalFinanceDB2.Data;

namespace PersonalFinanceDB2
{
    public class ReceiptProcessor
    {
        public TempReceipt TempReceipt { get; set; }
        public List<TempDetails> TempDetails { get; set; }
        public ReceiptProcessor()
        {
            TempDetails = new List<TempDetails>();
        }
        public void CreateReceiptByConsole()
        {
            EnterTempReceipt();
            while (true)
            {
                TempDetails.Add(EnterTempDetails());
                Console.WriteLine("New product?");
                if (Console.ReadLine() == "n")
                {
                    break;
                }
            }
            while (true)
            {
                Console.WriteLine("Check data:");
                ShowReceiptData();
                Console.WriteLine("Save data?");
                Console.WriteLine("y/n or e (edit)");
                string action = Console.ReadLine();
                if (action == "y")
                {
                    CheckIfProductsExist();
                    ProcessReceipt();
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
                        EnterTempReceipt();
                    }
                    else
                    {
                        TempDetails[line - 2] = EnterTempDetails();
                    }
                }
            }
        }
        public void CreateReceiptByCSV(string filePath)
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
                                //receiptID = FinanceRepository.AddBriefReceiptInfo(tempReceipt);
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
                                //AddPurchaseDetail(dbContext, tempDetails, receiptID);
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
        private void EnterTempReceipt()
        {
            TempReceipt.StoreName = GetInput("Store name:");
            TempReceipt.DateTime = GetInput("Date and time:");
            TempReceipt.TotalAmount = GetInput("Total amount:");
        }
        private static TempDetails EnterTempDetails()
        {
            TempDetails tempDetails = new TempDetails();
            tempDetails.ProductName = GetInput("Product name:");
            tempDetails.Quantity = GetInput("Quantity:");
            tempDetails.Amount = GetInput("Amount:");
            tempDetails.Category = GetInput("Category:");
            if (tempDetails.Category != string.Empty)
            {
                tempDetails.Subcategory = GetInput("Subcategory:");
            }
            return tempDetails;
        }
        private static string GetInput(string message)
        {
            Console.WriteLine(message);
            return Console.ReadLine();
        }
        private void ShowReceiptData()
        {
            Console.WriteLine($"\tStore name: {TempReceipt.StoreName}");
            Console.WriteLine($"\tDate and time: {TempReceipt.DateTime}");
            Console.WriteLine($"\tTotal amount: {TempReceipt.TotalAmount}");
            foreach (var item in TempDetails)
            {
                Console.WriteLine($"\tProduct name: {item.ProductName} | Quantity: {item.Quantity} | Amount: {item.Amount} | Category: {item.Category} | Subcategory: {item.Subcategory}");
            }
        }
        private void ProcessReceipt()
        {
            using (PersonalFinanceDbContext dbContext = new PersonalFinanceDbContext())
            {
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        FinanceRepository financeRepository = new FinanceRepository(dbContext);
                        financeRepository.AddFullReceipt(TempReceipt, TempDetails);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw new TransactionAbortedException("Transaction aborted");
                    }
                }
            }
        }
        public void CheckIfProductsExist()
        {
            using (PersonalFinanceDbContext dbContext = new PersonalFinanceDbContext())
            {
                for (int i = 0; i < TempDetails.Count; i++)
                {
                    if (dbContext.Products.FirstOrDefault(p => p.Name == TempDetails[i].ProductName) == null)
                    {
                        if (string.IsNullOrEmpty(TempDetails[i].Category) || string.IsNullOrEmpty(TempDetails[i].Subcategory))
                        {
                            Console.WriteLine($"Product \"{TempDetails[i].ProductName}\" not found in database");
                            Console.WriteLine("Enter category name:");
                            TempDetails[i].Category = Console.ReadLine();
                            Console.WriteLine("Enter subcategory name:");
                            TempDetails[i].Subcategory = Console.ReadLine();
                        }
                    }
                }
            }
        }
    }
}
