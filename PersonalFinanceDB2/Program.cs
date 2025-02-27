using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using PersonalFinanceDB2;



namespace PersonalFinanceDB2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ReceiptProcessor receiptProcessor = new ReceiptProcessor();
            string csvPath = "E:\\Projects\\PersonalFinance\\Receipts.csv";

            Console.WriteLine("Personal Finance Data Base v1.0");
            Console.WriteLine($"To process the CSV file it must be located here: {csvPath}");
            Console.WriteLine("Select an action:");
            Console.WriteLine("1 - Create Receipt By Console");
            Console.WriteLine("2 - Create Receipt By CSV");
            int action = 0;
            action = int.Parse(Console.ReadLine());

            if (action == 1)
            {
                receiptProcessor.CreateReceiptByConsole();
            } 
            else if (action == 2)
            {
                receiptProcessor.CreateReceiptByCSV(csvPath);
            }

            Console.WriteLine("Press ENTER to exit");
            Console.ReadLine();
        }
    }
}