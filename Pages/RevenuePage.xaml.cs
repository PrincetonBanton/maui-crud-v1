using MauiCrud.Models;
using MauiCrud.Services;

namespace MauiCrud.Pages
{
    public partial class RevenuePage : ContentPage
    {
        private readonly ApiService _apiService = new();
        private Revenue _currentRevenue = new();

        public RevenuePage(Revenue revenue = null)
        {
            InitializeComponent();
            _currentRevenue = revenue ?? new Revenue();
            BindRevenueToForm();
            LoadRevenues();
        }

        private void BindRevenueToForm()
        {
            descriptionEntry.Text = _currentRevenue.Description;
            amountEntry.Text = _currentRevenue.Amount > 0 ? _currentRevenue.Amount.ToString() : string.Empty;
            clientEntry.Text = _currentRevenue.Client;
            arrivedDatePicker.Date = _currentRevenue.ArrivedDate != default ? _currentRevenue.ArrivedDate : DateTime.Now;
        }

        private async void LoadRevenues()
            => revenueListView.ItemsSource = await _apiService.GetRevenuesAsync();

        private async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            _currentRevenue.Description = descriptionEntry.Text;
            _currentRevenue.Amount = decimal.TryParse(amountEntry.Text, out var amount) ? amount : 0;
            _currentRevenue.Client = clientEntry.Text;
            _currentRevenue.ArrivedDate = arrivedDatePicker.Date;

            bool success = _currentRevenue.RevenueId == 0
                ? await _apiService.CreateRevenueAsync(_currentRevenue)
                : await _apiService.UpdateRevenueAsync(_currentRevenue);

            await DisplayAlert(success ? "Success" : "Error", success ? "Revenue saved." : "Failed to save revenue.", "OK");

            if (success)
            {
                ClearForm();
                LoadRevenues();
            }
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
                if (await _apiService.DeleteRevenueAsync(selectedRevenue.RevenueId))
                {
                    await DisplayAlert("Success", "Revenue deleted.", "OK");
                    LoadRevenues();
                }
                else
                {
                    await DisplayAlert("Error", "Failed to delete revenue.", "OK");
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
