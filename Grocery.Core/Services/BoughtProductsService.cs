
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class BoughtProductsService : IBoughtProductsService
    {
        private readonly IGroceryListItemsRepository _groceryListItemsRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IProductRepository _productRepository;
        private readonly IGroceryListRepository _groceryListRepository;
        public BoughtProductsService(IGroceryListItemsRepository groceryListItemsRepository, IGroceryListRepository groceryListRepository, IClientRepository clientRepository, IProductRepository productRepository)
        {
            _groceryListItemsRepository=groceryListItemsRepository;
            _groceryListRepository=groceryListRepository;
            _clientRepository=clientRepository;
            _productRepository=productRepository;
        }
        public List<BoughtProducts> Get(int? productId)
        {
            //throw new NotImplementedException();
            // Return empty list if no productId was given
            var result = new List<BoughtProducts>();
            if (productId == null) return result;

            // Load all clients, lists and items from repositories
            var clients = _clientRepository.GetAll();
            var groceryLists = _groceryListRepository.GetAll();
            var items = _groceryListItemsRepository.GetAll();

            // Loop through all grocery list items
            foreach (var item in items)
            {
                // Only look at items that match the requested productId
                if (item.ProductId == productId)
                {
                    // Get the product from repository
                    var product = _productRepository.Get(item.ProductId);

                    // Find the grocery list for this item
                    var list = groceryLists.FirstOrDefault(l => l.Id == item.GroceryListId);

                    // Find the client who owns this list
                    var client = clients.FirstOrDefault(c => c.Id == list.ClientId);

                    // If all three exist, add them to the result list
                    if (product != null && list != null && client != null)
                    {
                        result.Add(new BoughtProducts
                        {
                            Client = client,
                            GroceryList = list,
                            Product = product
                        });
                    }
                }
            }

            return result;
        }
    }
}
