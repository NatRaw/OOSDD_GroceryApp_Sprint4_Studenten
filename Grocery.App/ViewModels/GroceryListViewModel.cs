using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;
using Grocery.Core.Data.Repositories;

namespace Grocery.App.ViewModels
{
    public partial class GroceryListViewModel : BaseViewModel
    {
        public ObservableCollection<GroceryList> GroceryLists { get; set; }
        private readonly IGroceryListService _groceryListService;
        
        // Add Client property logged in client
        [ObservableProperty] private Client client;

        public GroceryListViewModel(IGroceryListService groceryListService) 
        {
            Title = "Boodschappenlijst";
            _groceryListService = groceryListService;
            GroceryLists = new(_groceryListService.GetAll());
            
            // set the current client here like logged in client  = user3 admin
             //client test client (not in repository) 
             Client = new Client(3, "Natan Tesfa", "user3@mail.com", "test") { Role = Role.Admin };
            //test for user if name changes when logging in
            //var clientRepo = new ClientRepository();
            //Client = clientRepo.Get(3);
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
        
        // This command is bound to the ToolbarItem in GroceryListsView when the user clicks the ToolbarItem, this method runs.
        [RelayCommand]
        public async Task ShowBoughtProducts()
        {
            // Check if a client is logged in and if their role is Admin
            if (client != null && Client.Role == Role.Admin)
            {
                // If the client is Admin, navigate to the BoughtProductsView
                await Shell.Current.GoToAsync(nameof(Views.BoughtProductsView));
            }
            // If the client is not an admin it does nothing
        }
    }
}
