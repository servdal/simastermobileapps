using Plugin.Firebase.CloudMessaging;
using System.Diagnostics;

namespace simastermobileapps;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        // Memulai proses setelah komponen halaman selesai diinisialisasi
        _ = GetTokenAndLoadWebViewAsync();
    }

    private async Task GetTokenAndLoadWebViewAsync()
    {
        try
        {
            // Meminta izin notifikasi (penting untuk Android 13+ dan iOS)
            await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();

            // Dapatkan Firebase Token
            var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();

            if (!string.IsNullOrEmpty(token))
            {
                // Cetak token ke log untuk verifikasi saat debugging
                Debug.WriteLine($"FCM Token: {token}");

                // Bentuk URL lengkap
                string url = $"https://sdtqdu.sch.id/cekandroid/{token}";
                Debug.WriteLine($"Loading URL: {url}");

                // Muat URL ke dalam WebView
                MyWebView.Source = new UrlWebViewSource { Url = url };
            }
            else
            {
                await DisplayAlert("Gagal", "Tidak dapat mengambil Firebase ID. Pastikan koneksi internet stabil.", "OK");
                // Sembunyikan loading jika gagal
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
            await DisplayAlert("Error", $"Terjadi kesalahan: {ex.Message}", "OK");
            // Sembunyikan loading jika terjadi error
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    // Event handler untuk menyembunyikan ActivityIndicator setelah halaman web selesai dimuat
    private void WebView_Navigated(object sender, WebNavigatedEventArgs e)
    {
        LoadingIndicator.IsRunning = false;
        LoadingIndicator.IsVisible = false;
    }
}