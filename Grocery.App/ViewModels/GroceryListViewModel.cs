using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;

namespace Grocery.App.ViewModels
{
    public partial class GroceryListViewModel : BaseViewModel
    {
        public ObservableCollection<GroceryList> GroceryLists { get; set; }
        public ObservableCollection<Client> Clients { get; set; }

        [ObservableProperty]
        private Client currentClient;


        private readonly IGroceryListService _groceryListService;
        private readonly IClientService _clientService;

        public GroceryListViewModel(IGroceryListService groceryListService, IClientService clientService)
        {
            Title = "Boodschappenlijst";
            _groceryListService = groceryListService;
            _clientService = clientService;
            GroceryLists = new(_groceryListService.GetAll());
            Clients = new(_clientService.GetAll());

            var email = Preferences.Get("UserEmail", string.Empty);
            CurrentClient = Clients.FirstOrDefault(c => c.EmailAddress == email);
        }

        [RelayCommand]
        public async Task SelectGroceryList(GroceryList groceryList)
        {
            Dictionary<string, object> paramater = new() { { nameof(GroceryList), groceryList } };
            await Shell.Current.GoToAsync($"{nameof(Views.GroceryListItemsView)}?Titel={groceryList.Name}", true, paramater);
        }
        public override void OnAppearing()
        {
            base.OnAppearing();
            GroceryLists = new(_groceryListService.GetAll());
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            GroceryLists.Clear();
        }

        //In GroceryListViewModel maak je de methode ShowBoughtProducts(). Als de Client de rol admin heeft dan navigeer je naar BoughtProductsView. Anders doe je niets.
        [RelayCommand]
        public async Task ShowBoughtProducts()
        {
            // Assuming you have a way to get the current client's role
            var email = Preferences.Get("UserEmail", string.Empty);
            var currentClientRole = _clientService.Get(email);
            if (currentClientRole?.UserRole == Client.Role.Admin)
            {
                await Shell.Current.GoToAsync(nameof(Views.BoughtProductsView));
            }
        }
    }
}
