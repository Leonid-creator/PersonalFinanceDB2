using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinanceDB2.Data
{
    public class Subcategory
    {
        public int SubcategoryID { get; set; }
        public string Name { get; set; }
        public int CategoryID { get; set; }
        public ICollection<Product> Products { get; set; }
        public Category Category { get; set; }
    }
}
