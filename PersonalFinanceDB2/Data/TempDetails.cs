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
        public string Quantity { get; set; }
        public string Amount { get; set; }
        public string Category { get; set; }
        public string Subcategory { get; set; }
    }
}
