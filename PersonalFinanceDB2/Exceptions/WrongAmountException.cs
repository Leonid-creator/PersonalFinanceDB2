using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinanceDB2.Exceptions
{
    internal class WrongAmountException : Exception
    {
        public WrongAmountException() { }
        public WrongAmountException(string message) 
        {
            Console.WriteLine(message);
        }
    }
}
