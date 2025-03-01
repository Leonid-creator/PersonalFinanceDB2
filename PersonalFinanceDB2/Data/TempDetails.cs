using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinanceDB2.Data
{
    public class TempDetails
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public string Subcategory { get; set; }
    }
}
