using Lamina.ViewModels;
using Lamina.Contracts.Services;
using Lamina.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;
using System.Text.Json;

namespace Lamina.Views;

public sealed partial class ShellPage : Page
{
    public ShellViewModel ViewModel { get; }

    public ShellPage(ShellViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();

        ViewModel.NavigationService.Frame = NavigationFrame;
        ViewModel.NavigationViewService.Initialize(NavigationViewControl);

        App.MainWindow.ExtendsContentIntoTitleBar = true;
        App.MainWindow.SetTitleBar(AppTitleBar);
        AppTitleBarText.Text = "Lamina ✦";

        // Start on the default Calculator
        ViewModel.NavigationService.NavigateTo(typeof(CalculatorViewModel).FullName);

        RefreshImportedList();
        App.MainWindow.Activated += MainWindow_Activated;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        TitleBarHelper.UpdateTitleBar(RequestedTheme);
        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu));
        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.GoBack));
    }

    private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
    {
        App.AppTitlebar = AppTitleBarText as UIElement;
    }

    private void NavigationViewControl_DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
    {
        AppTitleBar.Margin = new Thickness()
        {
            Left = sender.CompactPaneLength * (sender.DisplayMode == NavigationViewDisplayMode.Minimal ? 2 : 1),
            Top = AppTitleBar.Margin.Top,
            Right = AppTitleBar.Margin.Right,
            Bottom = AppTitleBar.Margin.Bottom
        };
    }

    // Handles sidebar item selection
    private void NavigationViewControl_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (args.InvokedItemContainer != null)
        {
            var tag = args.InvokedItemContainer.Tag?.ToString();

            // Logic for navigating to the Manager
            if (tag == "DyNamoManager")
            {
                NavigationFrame.Navigate(typeof(DyNamoMangerPage));
            }
        }
    }

    public async void OnAddScriptieTapped(object sender, TappedRoutedEventArgs e)
    {
        var picker = new FileOpenPicker();
        var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(picker, hwnd);

        picker.FileTypeFilter.Add(".lamina");
        picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

        var file = await picker.PickSingleFileAsync();
        if (file != null)
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var folder = await localFolder.CreateFolderAsync("Scripties", CreationCollisionOption.OpenIfExists);

            await file.CopyAsync(folder, file.Name, NameCollisionOption.ReplaceExisting);

            RefreshImportedList();

            if (NavigationFrame.Content is DyNamoMangerPage manager)
            {
                manager.LoadScripties();
            }
        }
    }

    public async void RefreshImportedList()
    {
        try
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var folder = await localFolder.CreateFolderAsync("Scripties", CreationCollisionOption.OpenIfExists);
            var files = await folder.GetFilesAsync();

            int headerIndex = NavigationViewControl.MenuItems.IndexOf(ImportedHeader);
            int addButtonIndex = NavigationViewControl.MenuItems.IndexOf(AddScriptieButton);

            if (headerIndex == -1 || addButtonIndex == -1) return;

            // Clear dynamic items
            int itemsToRemove = addButtonIndex - headerIndex - 1;
            for (int i = 0; i < itemsToRemove; i++)
            {
                NavigationViewControl.MenuItems.RemoveAt(headerIndex + 1);
            }

            int currentInjectionIndex = headerIndex + 1;
            foreach (var file in files)
            {
                if (!file.FileType.Equals(".lamina", StringComparison.OrdinalIgnoreCase)) continue;

                string json = await FileIO.ReadTextAsync(file);
                var script = JsonSerializer.Deserialize<LaminaScript>(json);
                string displayName = script?.Metadata?.Name ?? file.DisplayName.Replace(".lamina", "");

                var newItem = new NavigationViewItem()
                {
                    Content = displayName,
                    Icon = new FontIcon
                    {
                        Glyph = "\uEA86",
                        FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Segoe Fluent Icons")
                    },
                    Tag = file.Path
                };

                // Context Menu
                var flyout = new MenuFlyout();
                var removeMenuItem = new MenuFlyoutItem { Text = "Remove", Icon = new SymbolIcon(Symbol.Delete) };
                removeMenuItem.Click += async (s, e) => {
                    await file.DeleteAsync();
                    RefreshImportedList();
                };

                flyout.Items.Add(removeMenuItem);
                newItem.ContextFlyout = flyout;

                newItem.Tapped += (s, e) => {
                    NavigationFrame.Navigate(typeof(DyNamoPage), file.Path);
                };

                NavigationViewControl.MenuItems.Insert(currentInjectionIndex, newItem);
                currentInjectionIndex++;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Dynamo Refresh Error: {ex.Message}");
        }
    }

    private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
    {
        var keyboardAccelerator = new KeyboardAccelerator() { Key = key };
        if (modifiers.HasValue) keyboardAccelerator.Modifiers = modifiers.Value;
        keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;
        return keyboardAccelerator;
    }

    private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        var navigationService = App.GetService<INavigationService>();
        args.Handled = navigationService.GoBack();
    }
}