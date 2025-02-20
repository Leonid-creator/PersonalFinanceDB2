using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinanceDB2.Exceptions
{
    internal class DuplicateReceiptException : Exception
    {
        public DuplicateReceiptException(string message)
        {
            Console.WriteLine(message);
        }
        public DuplicateReceiptException() { }
    }
}
