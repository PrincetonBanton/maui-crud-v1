using MauiCrud.Models;
using MauiCrud.Services;

namespace MauiCrud.Pages;

public partial class ExpenseCategoryPage : ContentPage
{
    private readonly ApiService _apiService = new();
    private ExpenseCategory _currentCategory = new();

    public ExpenseCategoryPage()
    {
        InitializeComponent();
        LoadExpenseCategories();
    }

    private async void LoadExpenseCategories()
    {
        var categories = await _apiService.GetExpenseCategoriesAsync();
        expenseCategoriesListView.ItemsSource = categories ?? new List<ExpenseCategory>();
    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(nameEntry.Text) || string.IsNullOrWhiteSpace(descriptionEntry.Text))
        {
            await DisplayAlert("Validation Error", "Name and description cannot be empty.", "OK");
            return;
        }

        _currentCategory.Name = nameEntry.Text;
        _currentCategory.Description = descriptionEntry.Text;

        bool success = _currentCategory.ExpenseCategoryId == 0
            ? await _apiService.CreateExpenseCategoryAsync(_currentCategory)
            : await _apiService.UpdateExpenseCategoryAsync(_currentCategory);

        await DisplayAlert(success ? "Success" : "Error", success ? "Category saved successfully." : "Failed to save category.", "OK");

        if (success)
        {
            ClearForm();
            LoadExpenseCategories();
        }
    }

    private async void ExpenseCategoriesListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is not ExpenseCategory selectedCategory) return;

        string action = await DisplayActionSheet("Action", "Cancel", null, "Edit", "Delete");

        if (action == "Edit")
        {
            _currentCategory = selectedCategory;
            nameEntry.Text = selectedCategory.Name;
            descriptionEntry.Text = selectedCategory.Description;
        }
        else if (action == "Delete" && await DisplayAlert("Confirm Delete", $"Delete '{selectedCategory.Name}'?", "Yes", "No"))
        {
            if (await _apiService.DeleteExpenseCategoryAsync(selectedCategory.ExpenseCategoryId))
            {
                await DisplayAlert("Success", "Category deleted.", "OK");
                LoadExpenseCategories();
            }
            else
            {
                await DisplayAlert("Error", "Failed to delete category.", "OK");
            }
        }

        ((ListView)sender).SelectedItem = null;
    }

    private void ClearForm()
    {
        _currentCategory = new ExpenseCategory();
        nameEntry.Text = string.Empty;
        descriptionEntry.Text = string.Empty;
    }

    private async void OnAddExpenseButtonClicked(object sender, EventArgs e)
        => await Navigation.PushAsync(new ExpensePage());

    private async void OnAddRevenueButtonClicked(object sender, EventArgs e)
        => await Navigation.PushAsync(new RevenuePage());
}
