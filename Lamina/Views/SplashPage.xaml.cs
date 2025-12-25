using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using WinRT.Interop;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Navigation;

namespace Lamina.Views;

public sealed partial class SplashPage : Page
{
    public SplashPage()
    {
        this.InitializeComponent();
        this.Loaded += SplashPage_Loaded;
    }

    private void SplashPage_Loaded(object sender, RoutedEventArgs e)
    {
        // Get the Window handle for the current window
        var hWnd = WindowNative.GetWindowHandle(App.MainWindow);
        // Get the AppWindow from the window handle
        var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
        AppWindow appWindow = AppWindow.GetFromWindowId(windowId);
        if (appWindow.TitleBar is not null)
        {
            appWindow.TitleBar.ExtendsContentIntoTitleBar = true;

            // Customize title bar buttons to follow the app theme
            var titleBar = appWindow.TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;            
        }

        // Start the fade-in animation
        var fadeInStoryboard = (Storyboard)this.Resources["FadeInStoryboard"];
        fadeInStoryboard.Begin();
    }

    public async Task StartFadeOutAndCloseAsync()
    {
        // Start the fade-out animation
        var fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard"];
        fadeOutStoryboard.Begin();

        // Wait for the duration of the fade-out animation
        await Task.Delay(400);

        // Close the page or navigate away
        Frame.Navigate(typeof(ShellPage));
    }

    protected async override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
        e.Cancel = true; // Cancel the navigation to allow the fade-out animation to play
        await StartFadeOutAndCloseAsync();
        e.Cancel = false; // Allow the navigation to proceed after the animation
    }
}
