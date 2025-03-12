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
using PersonalFinanceDB2.Exceptions;

namespace PersonalFinanceDB2
{
    public class ReceiptProcessor
    {
        public TempReceipt TempReceipt { get; set; }
        public List<TempDetails> TempDetails { get; set; }
        public ReceiptProcessor()
        {
            TempReceipt = new TempReceipt();
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
            bool isReadingReceiptInfo = true;
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                IgnoreBlankLines = false
            };

            var reader = new StreamReader(filePath);
            var csv = new CsvReader(reader, config);
            Receipt newReceipt = new Receipt();
            int detailsCounter = 0;

            csv.Read();
            while (csv.Read())
            {
                string[] fields = csv.Parser.Record;
                if (string.IsNullOrWhiteSpace(fields[0]))
                {
                    CheckIfProductsExist();
                    ProcessReceipt();
                    TempDetails.Clear();
                    detailsCounter = 0;
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
                    TempReceipt.StoreName = fields[0];
                    TempReceipt.DateTime = Convert.ToDateTime(fields[1]);
                    TempReceipt.TotalAmount = Convert.ToDecimal(fields[2]);
                    Console.WriteLine($"new receipt ({fields[0]})");          //just for test, delete it
                }
                else
                {
                    TempDetails.Add(new TempDetails() 
                    {
                        ProductName = fields[0],
                        Quantity = Convert.ToInt32(fields[1]),
                        Amount = Convert.ToDecimal(fields[2]),
                        Discount = Convert.ToDecimal(fields[3])
                    });
                    if (TempDetails[detailsCounter].Discount > 0)
                    {
                        throw new WrongDiscountException(TempDetails[detailsCounter].ProductName);
                    }
                    if (fields.Length == 6)
                    {
                        TempDetails[detailsCounter].Category = fields[4];
                        TempDetails[detailsCounter].Subcategory = fields[5];
                    }
                    Console.WriteLine($"new detail ({fields[0]})");          //just for test, delete it
                    detailsCounter++;
                }
            }
            CheckIfProductsExist();
            ProcessReceipt();
        }
        private void EnterTempReceipt()
        {
            TempReceipt.StoreName = GetInput("Store name:");
            TempReceipt.DateTime = Convert.ToDateTime(GetInput("Date and time:"));
            TempReceipt.TotalAmount = Convert.ToDecimal(GetInput("Total amount:"));
        }
        private static TempDetails EnterTempDetails()
        {
            TempDetails tempDetails = new TempDetails();
            tempDetails.ProductName = GetInput("Product name:");
            tempDetails.Quantity = Convert.ToInt32(GetInput("Quantity:"));
            tempDetails.Amount = Convert.ToDecimal(GetInput("Amount:"));
            string discount = GetInput("Discount:");
            if (discount.IsNullOrEmpty())
            {
                discount = "0";
            }
            tempDetails.Discount = Convert.ToDecimal(discount);
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
            CheckTotalAmount();
            CheckIfStoreExist();

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
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine(ex);
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
                        //Console.WriteLine($"Product \"{TempDetails[i].ProductName}\" not found in database");
                        if (string.IsNullOrEmpty(TempDetails[i].Category))
                        {
                            Console.WriteLine($"Category name for \"{TempDetails[i].ProductName}\":");
                            TempDetails[i].Category = Console.ReadLine();
                            if (TempDetails[i].Category == string.Empty)
                            {
                                TempDetails[i].Category = "UNCATEGORIZED";
                            }
                        }
                        CheckIfCategoryExist(i);
                        if (string.IsNullOrEmpty(TempDetails[i].Subcategory))
                        {
                            Console.WriteLine($"Subcategory name for \"{TempDetails[i].ProductName}\":");
                            TempDetails[i].Subcategory = Console.ReadLine();
                            if (TempDetails[i].Subcategory == string.Empty)
                            {
                                TempDetails[i].Subcategory = "UNCATEGORIZED";
                            }
                        }
                        CheckIfSubcategoryExist(i);
                    }
                }
            }
        }
        public bool CheckIfStoreExist()
        {
            using (PersonalFinanceDbContext dbContext = new PersonalFinanceDbContext())
            {
                if (dbContext.Stores.FirstOrDefault(s => s.Name == TempReceipt.StoreName) != null)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine($"Unknown store \"{TempReceipt.StoreName}\"");
                    Console.WriteLine("Add new store? (y/n)");
                    string option = Console.ReadLine();
                    if (option == "y")
                    {
                        FinanceRepository financeRepository = new FinanceRepository(dbContext);
                        financeRepository.AddNewStore(TempReceipt.StoreName);
                        return true;
                    }
                    return false;
                }
            }
        }
        public bool CheckIfCategoryExist(int prodNum)
        {
            using (PersonalFinanceDbContext dbContext = new PersonalFinanceDbContext())
            {
                //for (int i = 0; i < TempDetails.Count; i++)
                //{
                    if (dbContext.Categories.FirstOrDefault(c => c.Name == TempDetails[prodNum].Category) == null)
                    {
                        Console.WriteLine($"Unknown category \"{TempDetails[prodNum].Category}\"");
                        Console.WriteLine("Add new category? (y/n)");
                        string option = Console.ReadLine();
                        if (option == "y")
                        {
                            FinanceRepository financeRepository = new FinanceRepository(dbContext);
                            financeRepository.AddNewCategory(TempDetails[prodNum].Category);
                        }
                        else
                        {
                            return false;
                        }
                    }
                //}
                return true;
            }
        }
        public bool CheckIfSubcategoryExist(int prodNum)
        {
            using (PersonalFinanceDbContext dbContext = new PersonalFinanceDbContext())
            {
                FinanceRepository financeRepository = new FinanceRepository(dbContext);
                //for (int i = 0; i < TempDetails.Count; i++)
                //{
                    Category category = dbContext.Categories.FirstOrDefault(c => c.Name == TempDetails[prodNum].Category);
                    if(category == null)
                    {
                        throw new NullReferenceException($"Unknown category \"{TempDetails[prodNum].Category}\"");
                    }
                    Subcategory subcategory = dbContext.Subcategories.FirstOrDefault(s => s.Name == TempDetails[prodNum].Subcategory && s.CategoryID == category.CategoryID);
                    if (subcategory == null)
                    {
                        Console.WriteLine($"Unknown subcategory \"{TempDetails[prodNum].Subcategory}\"");
                        Console.WriteLine("Add new subcategory? (y/n)");
                        string option = Console.ReadLine();
                        if (option == "y")
                        {
                            financeRepository.AddNewSubcategory(TempDetails[prodNum].Category, TempDetails[prodNum].Subcategory);
                        }
                        else
                        {
                            return false;
                        }
                    }
                //}
                return true;
            }
        }
        public void CheckTotalAmount()
        {
            decimal sumAmountByDetails = TempDetails.Sum(s => s.Amount * s.Quantity + s.Discount);
            decimal diffAmount = TempReceipt.TotalAmount - sumAmountByDetails;
            if (diffAmount != 0)
            {
                throw new WrongAmountException(diffAmount);
            }
        }
    }
}
