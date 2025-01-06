using MauiCrud.Models;
using MauiCrud.Services;

namespace MauiCrud.Pages
{
    public partial class ExpensePage : ContentPage
    {
        private readonly ApiService _apiService;
        private Expense _expense; // To track the expense being edited
        public DateTime CurrentDate { get; set; } = DateTime.Now;

        public ExpensePage(Expense expense = null)
        {
            InitializeComponent();
            _apiService = new ApiService();

            if (expense != null)
            {
                _expense = expense;
                descriptionEntry.Text = expense.Description;
                amountEntry.Text = expense.Amount.ToString();
                expenseDatePicker.Date = expense.ExpenseDate;
                categoryPicker.SelectedItem = expense.ExpenseCategory;
            }
            else
            {
                _expense = new Expense();
            }

            LoadCategories();
            LoadExpenses();
        }

        private async void LoadCategories()
        {
            var categories = await _apiService.GetExpenseCategoriesAsync();
            categoryPicker.ItemsSource = categories;
        }

        private async void LoadExpenses()
        {
            var expenses = await _apiService.GetExpensesAsync();
            expenseListView.ItemsSource = expenses;
        }

        private async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            _expense.Description = descriptionEntry.Text;
            _expense.Amount = decimal.Parse(amountEntry.Text);
            _expense.ExpenseDate = expenseDatePicker.Date;
            _expense.ExpenseCategoryId = ((ExpenseCategory)categoryPicker.SelectedItem).ExpenseCategoryId;

            bool success;

            if (_expense.ExpenseId == 0)
            {
                // New Expense
                success = await _apiService.CreateExpenseAsync(_expense);
            }
            else
            {
                // Update Expense
                success = await _apiService.UpdateExpenseAsync(_expense);
            }

            if (success)
            {
                await DisplayAlert("Success", "Expense saved successfully.", "OK");
                ClearForm();
                LoadExpenses(); // Refresh the list of expenses
            }
            else
            {
                await DisplayAlert("Error", "Failed to save expense.", "OK");
            }
        }

        private async void ExpenseListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            // Get the tapped expense item
            if (e.Item is Expense selectedExpense)
            {
                // Show action sheet for editing or deleting
                var action = await DisplayActionSheet("Action", "Cancel", null, "Edit", "Delete");

                switch (action)
                {
                    case "Edit":
                        // Set the selected expense and populate the form
                        descriptionEntry.Text = selectedExpense.Description;
                        amountEntry.Text = selectedExpense.Amount.ToString();
                        expenseDatePicker.Date = selectedExpense.ExpenseDate;
                        categoryPicker.SelectedItem = selectedExpense.ExpenseCategory;
                        _expense = selectedExpense; // Track the expense being edited
                        break;

                    case "Delete":
                        // Confirm before deleting the expense
                        var confirm = await DisplayAlert("Confirm Delete", $"Are you sure you want to delete the expense '{selectedExpense.Description}'?", "Yes", "No");
                        if (confirm)
                        {
                            if (await _apiService.DeleteExpenseAsync(selectedExpense.ExpenseId))
                            {
                                await DisplayAlert("Success", "Expense deleted successfully.", "OK");
                                LoadExpenses(); // Reload the list of expenses
                            }
                            else
                            {
                                await DisplayAlert("Error", "Failed to delete expense.", "OK");
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
            descriptionEntry.Text = string.Empty;
            amountEntry.Text = string.Empty;
            expenseDatePicker.Date = DateTime.Now;
            categoryPicker.SelectedItem = null;
            _expense = null; // Clear the selected expense
        }
    }
}
