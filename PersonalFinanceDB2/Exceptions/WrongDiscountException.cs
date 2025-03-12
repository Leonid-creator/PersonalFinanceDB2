using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinanceDB2.Exceptions
{
    public class WrongDiscountException : Exception
    {
        public WrongDiscountException() { }
        public WrongDiscountException(string productName)
        {
            Console.WriteLine($"Error! Discount cannot take a positive value (Product with error - \"{productName})\"");
        }
    }
}
