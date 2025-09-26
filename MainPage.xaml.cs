using Plugin.Firebase.CloudMessaging;
using System.Diagnostics;

namespace simastermobileapps;

// Tambahkan 'partial' di sini
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
                // Gunakan variabel BaseUrl
                url = $"{BaseUrl}/cekandroid/{token}";
            }
            else
            {
                await DisplayAlert("Gagal", "Tidak dapat mengambil Firebase ID. Pastikan koneksi internet stabil.", "OK");
                // Gunakan variabel BaseUrl
                url = BaseUrl; // Fallback tanpa token
            }
            Debug.WriteLine($"Loading URL: {url}");
            MyWebView.Source = new UrlWebViewSource { Url = url };
            UrlLabel.Text = url;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
            await DisplayAlert("Error", $"Terjadi kesalahan: {ex.Message}", "OK");
            // Gunakan variabel BaseUrl
            string fallbackUrl = BaseUrl;
            MyWebView.Source = new UrlWebViewSource { Url = fallbackUrl };
            UrlLabel.Text = fallbackUrl;
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    // ... sisa kode Anda tidak perlu diubah ...
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
#if ANDROID
        Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
#elif IOS
        System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
#endif
    }
}