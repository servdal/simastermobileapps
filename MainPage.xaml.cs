using Plugin.Firebase.CloudMessaging;
using System.Diagnostics;

namespace simastermobileapps;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
		NavigationPage.SetHasNavigationBar(this, false);
        _ = GetTokenAndLoadWebViewAsync();
    }

    private async Task GetTokenAndLoadWebViewAsync()
    {
        try
        {
            await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
            var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();

            string url;
            if (!string.IsNullOrEmpty(token))
            {
                Debug.WriteLine($"FCM Token: {token}");
                url = $"https://sdtqdu.sch.id/cekandroid/{token}";
            }
            else
            {
                await DisplayAlert("Gagal", "Tidak dapat mengambil Firebase ID. Pastikan koneksi internet stabil.", "OK");
                url = "https://sdtqdu.sch.id/cekandroid/"; // Fallback tanpa token
            }
            Debug.WriteLine($"Loading URL: {url}");
            MyWebView.Source = new UrlWebViewSource { Url = url };
            UrlLabel.Text = url;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
            await DisplayAlert("Error", $"Terjadi kesalahan: {ex.Message}", "OK");
            // Fallback: tetap tampilkan WebView tanpa token
            string fallbackUrl = "https://sdtqdu.sch.id/";
            MyWebView.Source = new UrlWebViewSource { Url = fallbackUrl };
            UrlLabel.Text = fallbackUrl;
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    private void WebView_Navigated(object sender, WebNavigatedEventArgs e)
    {
        LoadingIndicator.IsRunning = false;
        LoadingIndicator.IsVisible = false;
        UrlLabel.Text = e.Url;
    }

    private void WebView_Navigating(object sender, WebNavigatingEventArgs e)
    {
        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;
        UrlLabel.Text = e.Url;
    }

    private void RefreshButton_Clicked(object sender, EventArgs e)
    {
        MyWebView.Reload();
    }

    private void BackButton_Clicked(object sender, EventArgs e)
    {
        if (MyWebView.CanGoBack)
            MyWebView.GoBack();
    }

    private void ExitButton_Clicked(object sender, EventArgs e)
    {
        // Untuk keluar aplikasi (Android/iOS), gunakan kode berikut:
#if ANDROID
        Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
#elif IOS
        // Tidak direkomendasikan untuk force close di iOS, bisa gunakan exit(0) jika benar-benar perlu
        System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
#endif
    }
}