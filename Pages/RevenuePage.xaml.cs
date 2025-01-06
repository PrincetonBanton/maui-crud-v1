using MauiCrud.Models;
using MauiCrud.Services;

namespace MauiCrud.Pages
{
    public partial class RevenuePage : ContentPage
    {
        private readonly ApiService _apiService;
        private Revenue _revenue; // To track the revenue being edited

        public DateTime CurrentDate { get; set; } = DateTime.Now;

        public RevenuePage(Revenue revenue = null)
        {
            InitializeComponent();
            _apiService = new ApiService();

            if (revenue != null)
            {
                _revenue = revenue;
                descriptionEntry.Text = revenue.Description;
                amountEntry.Text = revenue.Amount.ToString();
                clientEntry.Text = revenue.Client;
                arrivedDatePicker.Date = revenue.ArrivedDate;
            }
            else
            {
                _revenue = new Revenue();
            }

            LoadRevenues();
        }

        private async void LoadRevenues()
        {
            var revenues = await _apiService.GetRevenuesAsync();
            revenueListView.ItemsSource = revenues;
        }

        private async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            _revenue.Description = descriptionEntry.Text;
            _revenue.Amount = decimal.Parse(amountEntry.Text);
            _revenue.Client = clientEntry.Text;
            _revenue.ArrivedDate = arrivedDatePicker.Date;

            bool success;

            if (_revenue.RevenueId == 0)
            {
                // New Revenue
                success = await _apiService.CreateRevenueAsync(_revenue);
            }
            else
            {
                // Update Revenue
                success = await _apiService.UpdateRevenueAsync(_revenue);
            }

            if (success)
            {
                await DisplayAlert("Success", "Revenue saved successfully.", "OK");
                ClearForm();
                LoadRevenues(); // Refresh the list of revenues
            }
            else
            {
                await DisplayAlert("Error", "Failed to save revenue.", "OK");
            }
        }

        private async void RevenueListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            // Get the tapped revenue item
            if (e.Item is Revenue selectedRevenue)
            {
                // Show action sheet for editing or deleting
                var action = await DisplayActionSheet("Action", "Cancel", null, "Edit", "Delete");

                switch (action)
                {
                    case "Edit":
                        // Set the selected revenue and populate the form
                        descriptionEntry.Text = selectedRevenue.Description;
                        amountEntry.Text = selectedRevenue.Amount.ToString();
                        clientEntry.Text = selectedRevenue.Client;
                        arrivedDatePicker.Date = selectedRevenue.ArrivedDate;
                        _revenue = selectedRevenue; // Track the revenue being edited
                        break;

                    case "Delete":
                        // Confirm before deleting the revenue
                        var confirm = await DisplayAlert("Confirm Delete", $"Are you sure you want to delete the revenue '{selectedRevenue.Description}'?", "Yes", "No");
                        if (confirm)
                        {
                            if (await _apiService.DeleteRevenueAsync(selectedRevenue.RevenueId))
                            {
                                await DisplayAlert("Success", "Revenue deleted successfully.", "OK");
                                LoadRevenues(); // Reload the list of revenues
                            }
                            else
                            {
                                await DisplayAlert("Error", "Failed to delete revenue.", "OK");
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
            clientEntry.Text = string.Empty;
            arrivedDatePicker.Date = DateTime.Now;
            _revenue = null; // Clear the selected revenue
        }
    }
}
