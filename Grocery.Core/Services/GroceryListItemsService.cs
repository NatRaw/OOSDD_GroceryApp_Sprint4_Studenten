using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class GroceryListItemsService : IGroceryListItemsService
    {
        private readonly IGroceryListItemsRepository _groceriesRepository;
        private readonly IProductRepository _productRepository;

        public GroceryListItemsService(IGroceryListItemsRepository groceriesRepository, IProductRepository productRepository)
        {
            _groceriesRepository = groceriesRepository;
            _productRepository = productRepository;
        }

        public List<GroceryListItem> GetAll()
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int groceryListId)
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll().Where(g => g.GroceryListId == groceryListId).ToList();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            return _groceriesRepository.Add(item);
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            throw new NotImplementedException();
        }

        public GroceryListItem? Get(int id)
        {
            return _groceriesRepository.Get(id);
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            return _groceriesRepository.Update(item);
        }

        public List<BestSellingProducts> GetBestSellingProducts(int topX = 5)
        {
            //throw new NotImplementedException();
            // Get all grocery list items each item = product purchased with quantity
            var items = _groceriesRepository.GetAll();
            // Dictionary to count total sales per productId
            var productSales = new Dictionary<int, int>();

            // Count total sales per productId
            foreach (var item in items)
            {
                if (!productSales.ContainsKey(item.ProductId))          // Check if this ProductId is already in the dictionary.
                    productSales[item.ProductId] = 0;                   // If not, initialize it with 0 sold items.
                                                                        // Then add the current item's Amount to the total sold count.
                productSales[item.ProductId] += item.Amount;
            }

            // Build BestSellingProducts list looking up each product
            var bestSellers = new List<BestSellingProducts>(); 
            foreach (var kvp in productSales)
            {
                var product = _productRepository.Get(kvp.Key);
                bestSellers.Add(new BestSellingProducts(
                    product?.Id ?? 0,
                    product?.Name ?? "Unknown",
                    product?.Stock ?? 0,
                    kvp.Value,
                    0
                ));
            }

            // Sort by sales and take top X
            bestSellers = bestSellers
                .OrderByDescending(p => p.NrOfSells)
                .Take(topX)
                .ToList();

            // Add ranking, assign ranking 1 = bestselling, 2 = second, best. en so on
            for (int i = 0; i < bestSellers.Count; i++)
            {
                bestSellers[i].Ranking = i + 1;
            }

            return bestSellers;
        }

        private void FillService(List<GroceryListItem> groceryListItems)
        {
            foreach (GroceryListItem g in groceryListItems)
            {
                g.Product = _productRepository.Get(g.ProductId) ?? new(0, "", 0);
            }
        }
    }
}
