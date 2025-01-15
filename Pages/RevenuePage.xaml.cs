using MauiCrud.Models;
using MauiCrud.Services;
using MauiCrud.Pages;

namespace MauiCrud.Pages
{
    public partial class RevenuePage : ContentPage
    {
        private Revenue _currentRevenue = new();
        private bool _isInternetAvailable;
        private readonly ApiService _apiService = new();
        private readonly DatabaseService _databaseService = new();
      
        public RevenuePage(Revenue revenue = null)
        {
            InitializeComponent();
            CheckConnectivity();
            _currentRevenue = revenue ?? new Revenue();
            BindRevenueToForm();
            //LoadRevenues();
        }
        private async void CheckConnectivity()
        {
            await ConnectivityService.Instance.CheckAndUpdateConnectivityAsync();
            _isInternetAvailable = ConnectivityService.Instance.IsInternetAvailable;

            internetStatusLabel.Text = _isInternetAvailable ? "Online" : "Offline";
            internetStatusLabel.TextColor = _isInternetAvailable ? Colors.Green : Colors.Red;

            if (_isInternetAvailable)
            {
                var localRevenues = await _databaseService.GetRevenuesAsync();
                if (localRevenues.Any()) await MigrateLocalDataToApi(localRevenues);
                LoadOnlineData();
            }
            else
            {
                await DisplayAlert("Connectivity", "You are currently offline", "Ok");
                LoadOfflineData();
            }
        }

        private async Task MigrateLocalDataToApi(List<Revenue> localRevenues)
        {
            foreach (var revenue in localRevenues)
            {
                var apiRevenue = new Revenue
                {
                    RevenueId = 0, // Ensure a new record is created on the server
                    Description = revenue.Description,
                    Amount = revenue.Amount,
                    Client = revenue.Client,
                    ArrivedDate = revenue.ArrivedDate
                };

                try
                {
                    bool success = await _apiService.CreateRevenueAsync(apiRevenue);
                    if (success) await _databaseService.DeleteRevenueAsync(revenue.RevenueId);
                    else await DisplayAlert("Migration Error", $"Failed to migrate: {revenue.Description}.", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Failed to migrate expense: {revenue.Description}. Check logs for details.", "OK");
                }
            }
            await DisplayAlert("Migration Complete", "All local revenues have been migrated to the API.", "OK");
        }

        private void BindRevenueToForm()
        {
            descriptionEntry.Text = _currentRevenue.Description;
            amountEntry.Text = _currentRevenue.Amount > 0 ? _currentRevenue.Amount.ToString() : string.Empty;
            clientEntry.Text = _currentRevenue.Client;
            arrivedDatePicker.Date = _currentRevenue.ArrivedDate != default ? _currentRevenue.ArrivedDate : DateTime.Now;
        }
        private async void LoadOnlineData()
            => revenueListView.ItemsSource = await _apiService.GetRevenuesAsync();
        private async void LoadOfflineData()
            => revenueListView.ItemsSource = await _databaseService.GetRevenuesAsync();

        private async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            _currentRevenue.Description = descriptionEntry.Text;
            _currentRevenue.Amount = decimal.TryParse(amountEntry.Text, out var amount) ? amount : 0;
            _currentRevenue.Client = clientEntry.Text;
            _currentRevenue.ArrivedDate = arrivedDatePicker.Date;

            if (_isInternetAvailable)
            {
                var result = _currentRevenue.RevenueId == 0
                    ? _apiService.CreateRevenueAsync(_currentRevenue)
                    : _apiService.UpdateRevenueAsync(_currentRevenue);
                LoadOnlineData();
            }
            else
            {
                var result = _currentRevenue.RevenueId == 0
                    ? await _databaseService.SaveRevenueAsync(_currentRevenue)
                    : await _databaseService.UpdateRevenueAsync(_currentRevenue);
                LoadOfflineData();
            }
            await DisplayAlert("Success", "Revenue saved.", "OK");
            ClearForm();
        }

        private async void RevenueListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is not Revenue selectedRevenue) return;

            string action = await DisplayActionSheet("Action", "Cancel", null, "Edit", "Delete");
            if (action == "Edit")
            {
                _currentRevenue = selectedRevenue;
                BindRevenueToForm();
            }
            else if (action == "Delete" && await DisplayAlert("Confirm", "Delete this revenue?", "Yes", "No"))
            {
                if (_isInternetAvailable)
                {
                    var success = await _apiService.DeleteRevenueAsync(selectedRevenue.RevenueId);
                    LoadOnlineData();
                    await DisplayAlert(success ? "Success" : "Error", success ? "Revenue deleted." : "Failed to delete revenue.", "OK");
                }
                else
                {
                    //var success = await _databaseService.DeleteAllRevenuesAsync(););
                    var success = await _databaseService.DeleteRevenueAsync(selectedRevenue.RevenueId) > 0;
                    LoadOfflineData();
                    await DisplayAlert(success ? "Success" : "Error", success ? "Revenue deleted offline." : "Failed to delete revenue offline.", "OK");
                }
            }
            ((ListView)sender).SelectedItem = null;
        }

        private void ClearForm()
        {
            _currentRevenue = new Revenue();
            descriptionEntry.Text = string.Empty;
            amountEntry.Text = string.Empty;
            clientEntry.Text = string.Empty;
            arrivedDatePicker.Date = DateTime.Now;
        }
    }
}
