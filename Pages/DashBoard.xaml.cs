using MauiCrud.Services;

namespace MauiCrud.Pages;

public partial class DashBoard : ContentPage
{
    public DashBoard()
    {
        InitializeComponent();
        CheckConnectivity(); // Check connectivity when the page loads
    }

    private async void CheckConnectivity()
    {
        await DisplayAlert("Debug", "Entered CheckConnectivity method", "OK");
        await ConnectivityService.Instance.CheckAndUpdateConnectivityAsync();
    }
    private async void OnExpenseFrameTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ExpensePage());
    }

    private async void OnRevenueFrameTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RevenuePage());
    }

}