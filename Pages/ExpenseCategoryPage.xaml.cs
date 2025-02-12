using MauiCrud.Models;
using MauiCrud.Services;
using MauiCrud.Pages;

namespace MauiCrud.Pages
{
    public partial class ExpenseCategoryPage : ContentPage
    {
        private ExpenseCategory _currentExpenseCategory = new();
        private bool _isInternetAvailable;
        private readonly ApiService _apiService = new();
        private readonly DatabaseService _databaseService = new();

        public ExpenseCategoryPage(ExpenseCategory expenseCategory = null)
        {
            InitializeComponent();
            CheckConnectivity();
            _currentExpenseCategory = expenseCategory ?? new ExpenseCategory();
            BindExpenseToForm();
        }

        private async void CheckConnectivity()
        {
            await ConnectivityService.Instance.CheckAndUpdateConnectivityAsync();
            _isInternetAvailable = ConnectivityService.Instance.IsInternetAvailable;
            internetStatusLabel.Text = _isInternetAvailable ? "Online" : "Offline";
            internetStatusLabel.TextColor = _isInternetAvailable ? Colors.Green : Colors.Red;

            if (_isInternetAvailable)
            {
                //var localExpenseCategory = await _databaseService.GetExpenseCategoryAsync();
                //if (localExpenseCategory.Any()) await MigrateLocalDataToApi(localExpenseCategory);
                LoadOnlineData();
            }
            else
            {
                await DisplayAlert("Connectivity", "You are currently offline", "Ok");
                LoadOfflineData();
            }
        }

        private async Task MigrateLocalDataToApi(List<ExpenseCategory> localExpenseCategory)
        {
            foreach (var expenseCategory in localExpenseCategory)
            {
                var apiExpenseCategory = new ExpenseCategory
                {
                    ExpenseCategoryId = 0, // Ensure a new record is created on the server
                    Name = expenseCategory.Name,
                    Description = expenseCategory.Description,
                };

                try
                {
                    bool success = await _apiService.CreateExpenseCategoryAsync(apiExpenseCategory);
                    if (success) await _databaseService.DeleteExpenseCategoryAsync(expenseCategory.ExpenseCategoryId);
                    else await DisplayAlert("Migration Error", $"Failed to migrate: {expenseCategory.Description}.", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Failed to migrate expense: {expenseCategory.Description}. Check logs for details.", "OK");
                }
            }
            await DisplayAlert("Migration Complete", "All local expenses have been migrated to the API.", "OK");
        }

        private async void LoadOnlineData() => expenseCategoryListView.ItemsSource = await _apiService.GetExpenseCategoryAsync();
        private async void LoadOfflineData() => expenseCategoryListView.ItemsSource = await _databaseService.GetExpenseCategoryAsync();

        private async Task<bool> ValidateExpenseCategoryInput()
        {
            if (string.IsNullOrWhiteSpace(nameEntry.Text))
            {
                await DisplayAlert("Validation Error", "Category name is required.", "OK");
                return false;
            }

            if (string.IsNullOrWhiteSpace(descriptionEntry.Text))
            {
                await DisplayAlert("Validation Error", "Description is required.", "OK");
                return false;
            }

            return true;
        }

        private async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            if (!await ValidateExpenseCategoryInput())
                return;

            _currentExpenseCategory.Name = nameEntry.Text;
            _currentExpenseCategory.Description = descriptionEntry.Text;

            if (_isInternetAvailable)
            {
                var result = _currentExpenseCategory.ExpenseCategoryId == 0
                    ? await _apiService.CreateExpenseCategoryAsync(_currentExpenseCategory)
                    : await _apiService.UpdateExpenseCategoryAsync(_currentExpenseCategory);
                LoadOnlineData();
            }
            else
            {
                var result = _currentExpenseCategory.ExpenseCategoryId == 0
                    ? await _databaseService.SaveExpenseCategoryAsync(_currentExpenseCategory)
                    : await _databaseService.UpdateExpenseCategoryAsync(_currentExpenseCategory);
                LoadOfflineData();
            }

            await DisplayAlert("Success", "Expense category saved.", "OK");
            ClearForm();
        }

        private async void ExpenseCategoryListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is not ExpenseCategory selectedExpenseCategory) return;

            string action = await DisplayActionSheet("Action", "Cancel", null, "Edit", "Delete");
            if (action == "Edit")
            {
                _currentExpenseCategory = selectedExpenseCategory;
                BindExpenseToForm();
            }
            else if (action == "Delete" && await DisplayAlert("Confirm", "Delete this expense?", "Yes", "No"))
            {
                if (_isInternetAvailable)
                {
                    var success = await _apiService.DeleteExpenseCategoryAsync(selectedExpenseCategory.ExpenseCategoryId);
                    LoadOnlineData();
                    await DisplayAlert(success ? "Success" : "Error", success ? "Expense deleted." : "Failed to delete expense.", "OK");
                }
                else
                {
                    var success = await _databaseService.DeleteExpenseCategoryAsync(selectedExpenseCategory.ExpenseCategoryId) > 0;
                    LoadOfflineData();
                    await DisplayAlert(success ? "Success" : "Error", success ? "Expense deleted offline." : "Failed to delete expense offline.", "OK");
                }
            }
            ((ListView)sender).SelectedItem = null;
        }
        private void BindExpenseToForm()
        {
            nameEntry.Text = _currentExpenseCategory.Name;
            descriptionEntry.Text = _currentExpenseCategory.Description;
        }
        private void ClearForm()
        {
            _currentExpenseCategory = new ExpenseCategory();
            nameEntry.Text = string.Empty;
            descriptionEntry.Text = string.Empty;
        }
    }
}
