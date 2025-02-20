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

            receiptProcessor.CreateReceiptByConsole();

            //string filePath = "E:\\Projects\\PersonalFinance\\3Receipts.csv";
            //string filePath = "E:\\Projects\\PersonalFinance\\ReceiptLidl.csv";
            //string filePath = "E:\\Projects\\PersonalFinance\\ReceiptTesco.csv";
            //string filePath = "E:\\Projects\\PersonalFinance\\ReceiptPenneys.csv";
            //string filePath = "E:\\Projects\\PersonalFinance\\WrongReceiptPenneys.csv";
            //FinanceDb.CreateReceiptByCSV(filePath);

        }
    }
}

//to do:
//1. Rebuild CreateReceiptByCSV() method
//