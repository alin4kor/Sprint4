
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
            if (!productId.HasValue) throw new ArgumentNullException(nameof(productId));

            var boughtProducts = new List<BoughtProducts>();

            var groceryListItems = _groceryListItemsRepository.GetAll().Where(item => item.ProductId == productId.Value);
            foreach (var item in groceryListItems)
            {
                var client = _clientRepository.Get(item.Id);
                var groceryList = _groceryListRepository.Get(item.GroceryListId);
                var product = _productRepository.Get(item.ProductId);

                if (client != null && groceryList != null && product != null)
                {
                    boughtProducts.Add(new BoughtProducts(client, groceryList, product));
                }
            }

            return boughtProducts;
        }
    }
}
