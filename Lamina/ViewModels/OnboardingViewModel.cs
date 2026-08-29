using CommunityToolkit.Mvvm.ComponentModel;
using Lamina.Contracts.Services;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;

namespace Lamina.ViewModels;

public partial class OnboardingViewModel : ObservableRecipient // The BEAUTIFUL Onboarding Experience's 2nd Backend Code. (srsly)
{
    private readonly IMicaService _micaService;
    private readonly ILocalSettingsService _localSettingsService;

    [ObservableProperty]
    private int _selectedBackdropIndex;

    [ObservableProperty]
    private bool _isBackdropCardDisabled;

    [ObservableProperty]
    private string _backdropInfoText;

    public OnboardingViewModel(IMicaService micaService, ILocalSettingsService localSettingsService)
    {
        _micaService = micaService;
        _localSettingsService = localSettingsService;

        // Check Windows version and handle Windows 10 restrictions
        bool isWindows10 = Services.WindowsVersionService.IsWindows10();
        IsBackdropCardDisabled = isWindows10;
        BackdropInfoText = isWindows10 ? "This setting is for Windows 11 Only! :(" : string.Empty;

        // Sync initial backdrop index
        Task.Run(async () => {
            var savedIndex = await _localSettingsService.ReadSettingAsync<int?>("AppBackdropIndex") ?? 0;
            var finalIndex = isWindows10 ? 2 : savedIndex;
            App.MainWindow.DispatcherQueue.TryEnqueue(() => { 
                SelectedBackdropIndex = finalIndex;
                // Apply the backdrop immediately on UI thread
                _micaService.SetBackdrop(finalIndex);
            });
        });
    }

    partial void OnSelectedBackdropIndexChanged(int value) // Backdrop Switch in Slide 2 on "Customise" Screen.
    {
        // Prevent backdrop changes on Windows 10, but still apply Acrylic
        if (IsBackdropCardDisabled)
        {
            // Force back to Acrylic (index 2) and apply it
            SelectedBackdropIndex = 2;
            _micaService.SetBackdrop(2);
            _ = _micaService.SaveMicaSettingAsync(2);
            return;
        }
        
        _micaService.SetBackdrop(value);
        _ = _micaService.SaveMicaSettingAsync(value);
    }

    // Property for the ToggleSwitch in Slide 2
    public bool IsSplashEnabled // No
    {
        get => Windows.Storage.ApplicationData.Current.LocalSettings.Values["ShowSplash"] as bool? ?? true;
        set
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["ShowSplash"] = value;
            OnPropertyChanged(nameof(IsSplashEnabled));
        }
    }

    public int SelectedThemeIndex // Boring Stuff that manipulates the Data in Settings.
    {
        get => (int)App.GetService<IThemeSelectorService>().Theme;
        set
        {
            _ = App.GetService<IThemeSelectorService>().SetThemeAsync((ElementTheme)value);
            OnPropertyChanged(nameof(SelectedThemeIndex));
        }
    }
}