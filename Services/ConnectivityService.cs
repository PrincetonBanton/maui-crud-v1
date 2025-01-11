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
        }
    }
}

