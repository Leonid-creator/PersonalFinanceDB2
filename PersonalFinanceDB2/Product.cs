using System;

namespace PersonalFinanceDB2
{
    public class Product
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public int? CategoryID { get; set; }
        public ICollection<PurchaseDetail> PurchaseDetails { get; set; }
        public Category? Category { get; set; }
    }
}
