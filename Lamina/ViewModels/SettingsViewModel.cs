// File: ViewModels/SettingsViewModel.cs
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lamina.Contracts.Services;
using Lamina.Helpers;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.ApplicationModel;
using WinUIEx;

namespace Lamina.ViewModels;

public enum MicaEffectType
{
    None, Mica, MicaAlt
}

public partial class SettingsViewModel : ObservableRecipient
{
    private readonly IThemeSelectorService _themeSelectorService;
    private readonly IMicaService _micaService; // Inject IMicaService

    [ObservableProperty]
    private ElementTheme _elementTheme;

    [ObservableProperty]
    private string _versionDescription;

    [ObservableProperty]
    private bool _isMicaAltEnabled;

    // Default version if not found
    private static readonly Version DefaultVersion = new Version(11, 26100, 10, 0);
    public static Version AppVersion { get; private set; } = DefaultVersion;

    public ICommand SwitchThemeCommand
    {
        get;
    }
    public ICommand SetMicaEffectCommand
    {
        get;
    }

    public SettingsViewModel(IThemeSelectorService themeSelectorService, IMicaService micaService) // Update constructor
    {
        _themeSelectorService = themeSelectorService;
        _micaService = micaService;
        _elementTheme = _themeSelectorService.Theme;
        _versionDescription = GetVersionDescription();
        IsMicaAltEnabled = _micaService.IsMicaAltEnabled; // Load initial state

        SwitchThemeCommand = new RelayCommand<ElementTheme>(
            async (param) =>
            {
                if (ElementTheme != param)
                {
                    ElementTheme = param;
                    await _themeSelectorService.SetThemeAsync(param);                    
                }
            });

        SetMicaEffectCommand = new RelayCommand(SaveMicaSetting);

        this.PropertyChanged += SettingsViewModel_PropertyChanged;
    }

    private void SettingsViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IsMicaAltEnabled))
        {
            SaveMicaSetting();
        }
    }

    private async void SaveMicaSetting()
    {
        await _micaService.SaveMicaSettingAsync(IsMicaAltEnabled);
    }

    private static string GetVersionDescription()
    {
        Version version;

        if (RuntimeHelper.IsMSIX)
        {
            try
            {
                var packageVersion = Windows.ApplicationModel.Package.Current.Id.Version;
                version = new Version(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
            }
            catch
            {
                version = DefaultVersion;
            }
        }
        else
        {
            try
            {
                version = Assembly.GetExecutingAssembly().GetName().Version ?? DefaultVersion;
            }
            catch
            {
                version = DefaultVersion;
            }
        }
        AppVersion = version;
        return $"Lamina - v{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }
}