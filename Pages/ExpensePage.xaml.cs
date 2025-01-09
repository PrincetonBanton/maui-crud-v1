using MauiCrud.Models;
using MauiCrud.Services;

namespace MauiCrud.Pages
{
    public partial class ExpensePage : ContentPage
    {
        private bool _isInternetAvailable;
        private readonly ApiService _apiService = new();
        //private readonly LocalDbService _localDbService = new(); // Service for fetching from the local database
        private Expense _currentExpense = new();

        public ExpensePage(Expense expense = null)
        {
            InitializeComponent();
            CheckConnectivity(); // Check connectivity on page load
            _currentExpense = expense ?? new Expense();
            BindExpenseToForm();
        }

        private async void CheckConnectivity()
        {
            await ConnectivityService.Instance.CheckAndUpdateConnectivityAsync();
            _isInternetAvailable = ConnectivityService.Instance.IsInternetAvailable;

            if (_isInternetAvailable)
            {
                LoadCategories();
                LoadExpenses();
                return;
            }

            await DisplayAlert("Offline Mode", "Fetching data from local database.", "OK");
            // LoadCategoriesFromLocalDb();
            // LoadExpensesFromLocalDb();
        }


        private void BindExpenseToForm()
        {
            descriptionEntry.Text = _currentExpense.Description;
            amountEntry.Text = _currentExpense.Amount > 0 ? _currentExpense.Amount.ToString() : string.Empty;
            expenseDatePicker.Date = _currentExpense.ExpenseDate != default ? _currentExpense.ExpenseDate : DateTime.Now;
            categoryPicker.SelectedItem = _currentExpense.ExpenseCategory;
        }

        private async void LoadCategories()
            => categoryPicker.ItemsSource = await _apiService.GetExpenseCategoriesAsync();

        private async void LoadExpenses()
            => expenseListView.ItemsSource = await _apiService.GetExpensesAsync();

        //private async void LoadCategoriesFromLocalDb()
        //    => categoryPicker.ItemsSource = await _localDbService.GetExpenseCategoriesAsync();

        //private async void LoadExpensesFromLocalDb()
        //    => expenseListView.ItemsSource = await _localDbService.GetExpensesAsync();
        private async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            _currentExpense.Description = descriptionEntry.Text;
            _currentExpense.Amount = decimal.TryParse(amountEntry.Text, out var amount) ? amount : 0;
            _currentExpense.ExpenseDate = expenseDatePicker.Date;
            _currentExpense.ExpenseCategoryId = (categoryPicker.SelectedItem as ExpenseCategory)?.ExpenseCategoryId ?? 0;

            bool success;

            if (_isInternetAvailable)
            {
                success = _currentExpense.ExpenseId == 0
                    ? await _apiService.CreateExpenseAsync(_currentExpense)
                    : await _apiService.UpdateExpenseAsync(_currentExpense);
            }
            else
            {
                success = false;
                //success = _currentExpense.ExpenseId == 0
                //    ? await LocalDbService.CreateExpenseAsync(_currentExpense) // LocalDbService.CreateExpenseAsync should be implemented
                //    : await LocalDbService.UpdateExpenseAsync(_currentExpense); // LocalDbService.UpdateExpenseAsync should be implemented
            }

            await DisplayAlert(success ? "Success" : "Error", success ? "Expense saved." : "Failed to save expense.", "OK");

            if (success)
            {
                ClearForm();
                CheckConnectivity(); // Reload data after save
            }
        }


        private async void ExpenseListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is not Expense selectedExpense) return;

            string action = await DisplayActionSheet("Action", "Cancel", null, "Edit", "Delete");
            if (action == "Edit")
            {
                _currentExpense = selectedExpense;
                BindExpenseToForm();
            }
            else if (action == "Delete" && await DisplayAlert("Confirm", "Delete this expense?", "Yes", "No"))
            {
                if (await _apiService.DeleteExpenseAsync(selectedExpense.ExpenseId))
                {
                    await DisplayAlert("Success", "Expense deleted.", "OK");
                    CheckConnectivity(); // Reload data after delete
                }
                else
                {
                    await DisplayAlert("Error", "Failed to delete expense.", "OK");
                }
            }
            ((ListView)sender).SelectedItem = null;
        }

        private void ClearForm()
        {
            _currentExpense = new Expense();
            descriptionEntry.Text = string.Empty;
            amountEntry.Text = string.Empty;
            expenseDatePicker.Date = DateTime.Now;
            categoryPicker.SelectedItem = null;
        }
    }
}
