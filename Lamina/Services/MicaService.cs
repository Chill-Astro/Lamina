using Lamina.Contracts.Services;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace Lamina.Services;

public class MicaService : IMicaService
{
    private const string MicaSettingsKey = "AppBackdropIndex";
    private readonly ILocalSettingsService _localSettingsService;

    public MicaService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }

    public void SetBackdrop(int index)
    {
        if (App.MainWindow == null) return;

        // 0: Mica Alt, 1: Mica, 2: Acrylic, 3: None
        App.MainWindow.SystemBackdrop = index switch
        {
            0 => new MicaBackdrop { Kind = MicaKind.BaseAlt },
            1 => new MicaBackdrop { Kind = MicaKind.Base },
            2 => new DesktopAcrylicBackdrop(),
            3 => null, // Removes backdrop
            _ => new MicaBackdrop { Kind = MicaKind.BaseAlt }
        };
    }

    public async Task SaveMicaSettingAsync(int index)
    {
        await _localSettingsService.SaveSettingAsync(MicaSettingsKey, index);
    }

    public async Task LoadMicaSettingAsync()
    {
        var savedIndex = await _localSettingsService.ReadSettingAsync<int?>(MicaSettingsKey) ?? 0;
        SetBackdrop(savedIndex);
    }
}