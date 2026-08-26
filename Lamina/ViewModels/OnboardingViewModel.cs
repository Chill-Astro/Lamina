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

    public OnboardingViewModel(IMicaService micaService, ILocalSettingsService localSettingsService)
    {
        _micaService = micaService;
        _localSettingsService = localSettingsService;

        // Sync initial backdrop index
        Task.Run(async () => {
            var savedIndex = await _localSettingsService.ReadSettingAsync<int?>("AppBackdropIndex") ?? 0;
            App.MainWindow.DispatcherQueue.TryEnqueue(() => { SelectedBackdropIndex = savedIndex; });
        });
    }

    partial void OnSelectedBackdropIndexChanged(int value) // Backdrop Switch in Slide 2 on "Customise" Screen.
    {
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