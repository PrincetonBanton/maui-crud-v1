using MauiCrud.Models;
using MauiCrud.Services;

namespace MauiCrud;

public partial class MainPage : ContentPage
{
    private readonly ApiService _apiService;
    private ExpenseCategory _selectedCategory; // To track the category being edited
    private Expense _selectedExpense;
    public DateTime CurrentDate { get; set; } = DateTime.Now;

    public MainPage()
    {
        InitializeComponent();
        _apiService = new ApiService();
        LoadExpenseCategories();
        //LoadExpenses();
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

    private async void OnAddButtonClicked(object sender, EventArgs e)
    {
        var name = nameEntry.Text;
        var description = descriptionEntry.Text;

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description))
        {
            await DisplayAlert("Validation Error", "Name and description cannot be empty.", "OK");
            return;
        }

        if (_selectedCategory == null)
        {
            // Create a new category
            var newCategory = new ExpenseCategory { Name = name, Description = description };

            if (await _apiService.CreateExpenseCategoryAsync(newCategory))
            {
                await DisplayAlert("Success", "Category added successfully.", "OK");
                ClearForm();
                LoadExpenseCategories();
            }
            else
            {
                await DisplayAlert("Error", "Failed to add category.", "OK");
            }
        }
        else
        {
            // Update the existing category
            _selectedCategory.Name = name;
            _selectedCategory.Description = description;

            if (await _apiService.UpdateExpenseCategoryAsync(_selectedCategory))
            {
                await DisplayAlert("Success", "Category updated successfully.", "OK");
                ClearForm();
                LoadExpenseCategories();
            }
            else
            {
                await DisplayAlert("Error", "Failed to update category.", "OK");
            }

            // Clear the selected category after editing
            _selectedCategory = null;
        }
    }

    private async void expenseCategoriesListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        // Get the tapped item
        if (e.Item is ExpenseCategory selectedCategory)
        {
            // Show the action sheet
            var action = await DisplayActionSheet("Action", "Cancel", null, "Edit", "Delete");

            switch (action)
            {
                case "Edit":
                    // Set the selected category and populate the form
                    nameEntry.Text = selectedCategory.Name;
                    descriptionEntry.Text = selectedCategory.Description;
                    _selectedCategory = selectedCategory; // Track the category being edited
                    break;

                case "Delete":
                    // Confirm before deletion
                    var confirm = await DisplayAlert("Confirm Delete", $"Are you sure you want to delete {selectedCategory.Name}?", "Yes", "No");
                    if (confirm)
                    {
                        if (await _apiService.DeleteExpenseCategoryAsync(selectedCategory.ExpenseCategoryId))
                        {
                            await DisplayAlert("Success", "Category deleted successfully.", "OK");
                            LoadExpenseCategories(); // Reload the list
                        }
                        else
                        {
                            await DisplayAlert("Error", "Failed to delete category.", "OK");
                        }
                    }
                    break;

                default:
                    // Do nothing if "Cancel" is selected
                    break;
            }
        }

        // Deselect the tapped item
        ((ListView)sender).SelectedItem = null;
    }

    private void ClearForm()
    {
        nameEntry.Text = string.Empty;
        descriptionEntry.Text = string.Empty;
        _selectedCategory = null; // Clear the selected category
    }

    private async void OnAddExpenseButtonClicked(object sender, EventArgs e)
    {
        // Navigate to ExpenseDetails page to add a new expense
        await Navigation.PushAsync(new ExpenseDetailsPage());
    }
}

