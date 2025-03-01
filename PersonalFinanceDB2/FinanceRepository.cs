
using PersonalFinanceDB2.Data;

namespace PersonalFinanceDB2
{
    internal class FinanceRepository
    {
        private readonly PersonalFinanceDbContext DbContext;
        public FinanceRepository(PersonalFinanceDbContext context)
        {
            DbContext = context;
        }
        public void AddFullReceipt(TempReceipt tempReceipt, List<TempDetails> tempDetails)
        {
            int receiptID = AddBriefReceiptInfo(tempReceipt);
            if (receiptID == 0)
            {
                return;
            }
            for (int i = 0; i < tempDetails.Count; i++)
            {
                AddPurchaseDetail(tempDetails[i], receiptID);
            }
            DbContext.SaveChanges();
        }
        private int AddBriefReceiptInfo(TempReceipt tempReceipt)
        {
            if (tempReceipt.StoreName == null || tempReceipt.TotalAmount == null || tempReceipt.DateTime == null)
            {
                throw new ArgumentNullException("Exception occurred in AddBriefReceiptInfo");
            }
            else
            {
                Store store = DbContext.Stores.FirstOrDefault(s => s.Name == tempReceipt.StoreName);
                if (store == null)
                {
                    store = AddStore(tempReceipt.StoreName);
                }

                Receipt newReceipt = new Receipt()
                {
                    StoreID = store.StoreID,
                    DateTime = tempReceipt.DateTime,
                    TotalAmount = tempReceipt.TotalAmount
                };
                var existingReceipt = DbContext.Receipts.FirstOrDefault(r => r.StoreID == store.StoreID
                                                                        && r.DateTime == newReceipt.DateTime
                                                                        && r.TotalAmount == newReceipt.TotalAmount);

                if (existingReceipt == null)
                {
                    DbContext.Receipts.Add(newReceipt);
                    DbContext.SaveChanges();
                }
                else
                {
                    Console.WriteLine($"Receipt \"{newReceipt.DateTime} {store.Name}\" already exist");
                    Console.WriteLine("Press ENTER to continue");
                    Console.ReadLine();
                }
                return newReceipt.ReceiptID;
            }
        }
        private void AddPurchaseDetail(TempDetails tempDetails, int receiptID)
        {
            if (tempDetails.ProductName == null || tempDetails.Quantity == null || tempDetails.Amount == null)
            {
                throw new ArgumentNullException("Exception occurred in AddPurchaseDetail");
            }
            else
            {
                Product newProduct = DbContext.Products.FirstOrDefault(s => s.Name == tempDetails.ProductName);
                if (newProduct == null)
                {
                    newProduct = AddProduct(tempDetails);
                }

                PurchaseDetail newPurchaseDetail = new PurchaseDetail
                {
                    ReceiptID = receiptID,
                    ProductID = newProduct.ProductID,
                    Quantity = tempDetails.Quantity,
                    Amount = tempDetails.Amount
                };
                DbContext.PurchaseDetails.Add(newPurchaseDetail);
            }
        }
        private Store AddStore(string storeName)
        {
            if (storeName == null)
            {
                throw new ArgumentNullException("Exception occurred in AddStore");
            }
            else
            {
                Store newStore = new Store { Name = storeName };
                DbContext.Stores.Add(newStore);
                DbContext.SaveChanges();
                return newStore;
            }
        }
        private Product AddProduct(TempDetails tempDetails)
        {
            if (tempDetails.ProductName == null || tempDetails.Category == null || tempDetails.Subcategory == null)
            {
                throw new ArgumentNullException("Exception occurred in AddProduct");
            }
            else
            {
                Subcategory subcategory = DbContext.Subcategories.FirstOrDefault(s => s.Name == tempDetails.Subcategory);
                if (subcategory == null)
                {
                    subcategory = AddSubcategory(tempDetails.Subcategory, tempDetails.Category);
                }

                Product newProduct = new Product
                {
                    Name = tempDetails.ProductName,
                    SubcategoryID = subcategory.SubcategoryID,
                    CategoryID = subcategory.CategoryID
                };
                DbContext.Products.Add(newProduct);
                DbContext.SaveChanges();
                return newProduct;
            } 
        }
        public Subcategory AddSubcategory(string subcategory, string category)
        {
            if (subcategory == null || category == null)
            {
                throw new ArgumentNullException("Exception occurred in AddSubcategory");
            }
            else
            {
                Category newCategory = DbContext.Categories.FirstOrDefault(c => c.Name == category);
                if (newCategory == null)
                {
                    newCategory = AddCategory(category);
                }

                Subcategory newSubcategory = new Subcategory
                {
                    Name = subcategory,
                    CategoryID = newCategory.CategoryID
                };
                DbContext.Subcategories.Add(newSubcategory);
                DbContext.SaveChanges();
                return newSubcategory;
            }
        }
        public Category AddCategory(string category)
        {
            if (category == null)
            {
                throw new ArgumentNullException("Exception occurred in AddCategory");
            }
            else
            {
                Category newCategory = new Category { Name = category };
                DbContext.Categories.Add(newCategory);
                DbContext.SaveChanges();
                return newCategory;
            } 
        }
    }
}
