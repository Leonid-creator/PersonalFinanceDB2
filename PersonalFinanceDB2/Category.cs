using System;

namespace PersonalFinanceDB2
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
