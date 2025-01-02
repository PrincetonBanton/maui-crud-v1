using MauiCrud.Models;
using MauiCrud.Services;

namespace MauiCrud;

public partial class MainPage : ContentPage
{
    private readonly ApiService _apiService;

    public MainPage()
    {
        InitializeComponent();
        _apiService = new ApiService();
        LoadExpenseCategories();
    }

    private async void LoadExpenseCategories()
    {
        var categories = await _apiService.GetExpenseCategoriesAsync();

        // Ensure data is not null or empty
        if (categories != null && categories.Any())
        {
            expenseCategoriesListView.ItemsSource = categories;
            expenseCategoriesPicker.ItemsSource = categories;
        }
        else
        {
            await DisplayAlert("No Data", "No categories found.", "OK");
        }
    }

}
