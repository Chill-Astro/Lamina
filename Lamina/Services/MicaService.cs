// File: Services/MicaService.cs
using Lamina;
using Lamina.Contracts.Services;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using WinUIEx;

namespace Lamina.Services;

public class MicaService : IMicaService
{
    private const string MicaSettingsKey = "AppMicaAltEnabled";

    public bool IsMicaAltEnabled { get; set; } = true; // Default to Mica Alt

    private readonly ILocalSettingsService _localSettingsService;

    public MicaService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }

    public void ApplyMica(ElementTheme currentTheme)
    {
        if (App.MainWindow == null) return;

        if (IsMicaSupported() && IsMicaAltEnabled)
        {
            App.MainWindow.SystemBackdrop = new MicaBackdrop { Kind = MicaKind.BaseAlt };

        }
        else
        {
            App.MainWindow.SystemBackdrop = new DesktopAcrylicBackdrop();
        }
    }

    private bool IsMicaSupported()
    {
        // Implement the logic to check if Mica is supported
        // This is a placeholder implementation
        return true;
    }

    public async Task SaveMicaSettingAsync(bool isMicaAltEnabled)
    {
        IsMicaAltEnabled = isMicaAltEnabled;
        await _localSettingsService.SaveSettingAsync(MicaSettingsKey, isMicaAltEnabled);
        ApplyMica(App.MainWindow?.Content is FrameworkElement rootElement ? rootElement.RequestedTheme : ElementTheme.Default); // Apply on save
    }

    public async Task LoadMicaSettingAsync()
    {
        IsMicaAltEnabled = await _localSettingsService.ReadSettingAsync<bool?>(MicaSettingsKey) ?? true; // Default to true
        ApplyMica(App.MainWindow?.Content is FrameworkElement rootElement ? rootElement.RequestedTheme : ElementTheme.Default); // Apply on load
    }
}
