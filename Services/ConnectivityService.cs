using Microsoft.Maui.Networking;
using System.Threading.Tasks;

namespace MauiCrud.Services
{
    public class ConnectivityService
    {
        private static readonly ConnectivityService _instance = new ConnectivityService();

        public static ConnectivityService Instance => _instance;

        private ConnectivityService() { }

        public bool IsInternetAvailable { get; private set; } = false;

        public async Task CheckAndUpdateConnectivityAsync()
        {
            IsInternetAvailable = Connectivity.NetworkAccess == NetworkAccess.Internet;

            // Display the status alert
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                string status = IsInternetAvailable ? "connected to the Internet." : "offline.";
                await App.Current.MainPage.DisplayAlert("Connectivity Status", $"Internet Available: {IsInternetAvailable}\nYou are currently {status}", "OK");
            });
        }
    }
}

