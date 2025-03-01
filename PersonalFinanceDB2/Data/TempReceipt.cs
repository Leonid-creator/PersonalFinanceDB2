using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinanceDB2.Data
{
    public class TempReceipt
    {
        public string StoreName { get; set; }
        public DateTime DateTime { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
