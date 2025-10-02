using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
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
            List<GroceryListItem> items = GetAll();
            List < GroceryListItem > combinedItems = new List<GroceryListItem>();
            foreach (GroceryListItem item in items)
            {
                GroceryListItem? existingItem = combinedItems.FirstOrDefault(i => i.ProductId == item.ProductId);
                if (existingItem != null)
                {
                    existingItem.Amount += item.Amount;
                }
                else
                {
                    combinedItems.Add(new GroceryListItem(0, item.GroceryListId, item.ProductId, item.Amount) { Product = item.Product });
                }
            }
            List<BestSellingProducts> bestSellingProducts = combinedItems
                .OrderByDescending(i => i.Amount)
                .Take(topX)
                .Select(i => new BestSellingProducts(i.Product.Id, i.Product.Name, _productRepository.Get(i.Product.Id)?.Stock ?? 0, i.Amount, 0))
                .ToList();
            for (int i = 0; i < bestSellingProducts.Count; i++)
            {
                bestSellingProducts[i].Ranking = i + 1;
            }
            return bestSellingProducts;
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
