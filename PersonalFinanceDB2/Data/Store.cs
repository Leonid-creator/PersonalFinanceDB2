using System;

namespace PersonalFinanceDB2.Data
{
    public class Store
    {
        public int StoreID { get; set; }
        public string Name { get; set; }
        public ICollection<Receipt> Receipts { get; set; }
    }
}
