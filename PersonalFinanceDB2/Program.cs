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
using PersonalFinanceDB2.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;



namespace PersonalFinanceDB2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var configuration = new ConfigurationBuilder()
            //.SetBasePath(Directory.GetCurrentDirectory())
            //.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            //.Build();

            //var connectionString = configuration.GetConnectionString("DefaultConnection");

            //var services = new ServiceCollection();
            //services.AddDbContext<PersonalFinanceDbContext>(options =>
            //    options.UseSqlServer(connectionString));

            //var serviceProvider = services.BuildServiceProvider();

            //using (var context = serviceProvider.GetRequiredService<PersonalFinanceDbContext>())
            //{
            //    Console.WriteLine("Подключение к БД успешно!");
            //}

            //-----------------------------------------------------------------------------------------

            ReceiptProcessor receiptProcessor = new ReceiptProcessor();
            string appVersion = "v1.02";
            string csvPath = "E:\\Projects\\PersonalFinance\\Receipts.csv";

            Console.WriteLine($"Personal Finance Data Base {appVersion}");
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

            Console.WriteLine("Finish. Press ENTER to exit");
            Console.ReadLine();
        }
    }
}