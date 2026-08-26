using Microsoft.UI.Composition.SystemBackdrops;

namespace Lamina.Contracts.Services;

public interface IMicaService
{
    // The main method for our ComboBox logic
    void SetBackdrop(int index);

    // Persistence
    Task SaveMicaSettingAsync(int index);
    Task LoadMicaSettingAsync();
}