using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Lamina.ViewModels;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using Windows.UI.Core;
using Microsoft.UI.Xaml.Navigation;
using System.Windows.Input;
using Windows.Storage;

namespace Lamina.Views;

public sealed partial class AdvancedCalculatorPage : Page
{
    private bool _isDialogOpen;
    private const double MaxDisplayFontSize = 48;
    private const double MinDisplayFontSize = 14;
    public AdvancedCalculatorViewModel ViewModel { get; }

    public AdvancedCalculatorPage()
    {
        ViewModel = App.GetService<AdvancedCalculatorViewModel>();
        InitializeComponent();
        this.Loaded += (s, e) => this.Focus(FocusState.Programmatic);
        this.SizeChanged += AdvancedCalculatorPage_SizeChanged;
        ViewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == "IsEditing")
            {
                SetButtonsEnabled(!ViewModel.IsEditing);
            }
        };
        ViewModel.CalculationHistory.CollectionChanged += (s, e) => UpdateHistorySidebar();
        UpdateHistorySidebar();
    }

    private void AdvancedCalculatorPage_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        const double WideWindowThreshold = 600;
        
        if (e.NewSize.Width >= WideWindowThreshold)
        {
            HistorySidebar.Visibility = Visibility.Visible;
            // Adaptive width: 200px minimum, up to 280px based on window width
            double sidebarWidth = Math.Min(280, Math.Max(200, e.NewSize.Width * 0.25));
            HistorySidebarColumn.Width = new GridLength(sidebarWidth);
            HistoryButton.Visibility = Visibility.Collapsed;
        }
        else
        {
            HistorySidebar.Visibility = Visibility.Collapsed;
            HistorySidebarColumn.Width = new GridLength(0);
            HistoryButton.Visibility = Visibility.Visible;
        }
    }

    private void UpdateHistorySidebar()
    {
        HistorySidebarList.ItemsSource = ViewModel.CalculationHistory;
        
        // Show/hide empty state
        if (ViewModel.CalculationHistory == null || ViewModel.CalculationHistory.Count == 0)
        {
            NoHistorySidebarText.Visibility = Visibility.Visible;
            HistoryScrollViewer.Visibility = Visibility.Collapsed;
        }
        else
        {
            NoHistorySidebarText.Visibility = Visibility.Collapsed;
            HistoryScrollViewer.Visibility = Visibility.Visible;
        }
    }

    private void SetButtonsEnabled(bool enabled)
    {
        // Find all buttons in the buttons grid and enable/disable them
        var allButtons = new List<Button>();
        
        void FindButtons(DependencyObject parent)
        {
            if (parent is Button button)
            {
                allButtons.Add(button);
            }
            
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                FindButtons(child);
            }
        }
        
        if (ButtonsGrid != null)
        {
            FindButtons(ButtonsGrid);
        }
        
        foreach (var button in allButtons)
        {
            button.IsEnabled = enabled;
        }
        
        // Always keep the textbox enabled
        DisplayTextBox.IsEnabled = true;
    }

    private void DisplayTextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        ViewModel.IsEditing = true;
    }

    private void DisplayTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        ViewModel.IsEditing = false;
    }

    private async void AnimateClick(Button button)
    {
        if (button == null) return;
        VisualStateManager.GoToState(button, "Pressed", true);
        await Task.Delay(100);
        VisualStateManager.GoToState(button, "Normal", true);
    }

    private void AdvancedCalculatorPage_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        var shift = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down);
        var ctrl = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down);

        bool handled = true;
        Button targetButton = null;

        // If editing mode, only handle Enter and Escape, let TextBox handle other keys for typing
        if (ViewModel.IsEditing)
        {
            switch (e.Key)
            {
                case VirtualKey.Enter:
                    ViewModel.CalculateCommand.Execute(null);
                    e.Handled = true;
                    return;
                case VirtualKey.Escape:
                    ViewModel.ClearAllCommand.Execute(null);
                    e.Handled = true;
                    return;
                default:
                    // Don't handle - let TextBox process the key for normal typing
                    return;
            }
        }

        // Arrow keys for cursor movement
        if (e.Key == VirtualKey.Left)
        {
            ViewModel.MoveCursorLeftCommand.Execute(null);
            return;
        }
        else if (e.Key == VirtualKey.Right)
        {
            ViewModel.MoveCursorRightCommand.Execute(null);
            return;
        }

        // 1. Numbers
        if (e.Key >= VirtualKey.Number0 && e.Key <= VirtualKey.Number9 && !shift)
        {
            string num = (e.Key - VirtualKey.Number0).ToString();
            ViewModel.InputNumberCommand.Execute(num);
            targetButton = this.FindName("Btn" + num) as Button;
        }
        else if (e.Key >= VirtualKey.NumberPad0 && e.Key <= VirtualKey.NumberPad9)
        {
            string num = (e.Key - VirtualKey.NumberPad0).ToString();
            ViewModel.InputNumberCommand.Execute(num);
            targetButton = this.FindName("Btn" + num) as Button;
        }
        // 2. NCalc Operators, Brackets, and Actions
        else
        {
            switch (e.Key)
            {
                case VirtualKey.Number9 when shift:
                    ViewModel.InputNumberCommand.Execute("(");
                    targetButton = BtnLeftBracket;
                    break;
                case VirtualKey.Number0 when shift:
                    ViewModel.InputNumberCommand.Execute(")");
                    targetButton = BtnRightBracket;
                    break;
                case VirtualKey.Number6 when shift:
                    ViewModel.InputNumberCommand.Execute("Pow(");
                    break;

                case VirtualKey.Add:
                case (VirtualKey)187 when shift:
                    ViewModel.SetOperatorCommand.Execute("+");
                    targetButton = BtnAdd;
                    break;
                case VirtualKey.Subtract:
                case (VirtualKey)189:
                    ViewModel.SetOperatorCommand.Execute("-");
                    targetButton = BtnSubtract;
                    break;
                case VirtualKey.Multiply:
                case VirtualKey.Number8 when shift:
                    ViewModel.SetOperatorCommand.Execute("×");
                    targetButton = BtnMultiply;
                    break;
                case VirtualKey.Divide:
                case (VirtualKey)191:
                    ViewModel.SetOperatorCommand.Execute("÷");
                    targetButton = BtnDivide;
                    break;
                case VirtualKey.Enter:
                case (VirtualKey)187 when !shift:
                    ViewModel.CalculateCommand.Execute(null);
                    targetButton = BtnEqual;
                    break;
                case VirtualKey.Back:
                    ViewModel.BackspaceCommand.Execute(null);
                    targetButton = BtnBackspace;
                    break;
                case VirtualKey.Escape:
                case VirtualKey.C when !ctrl:
                    ViewModel.ClearAllCommand.Execute(null);
                    targetButton = BtnAC;
                    break;
                case VirtualKey.Decimal:
                case (VirtualKey)190:
                    ViewModel.InputDecimalCommand.Execute(null);
                    targetButton = BtnDecimal;
                    break;                
                case (VirtualKey)188:
                    ViewModel.InputCommaCommand.Execute(null);
                    targetButton = BtnComma;
                    break;
                case VirtualKey.H when ctrl:
                    OpenHistoryDialog();
                    break;
                default:
                    handled = false;
                    break;
            }
        }

        if (handled)
        {
            e.Handled = true;
            if (targetButton != null) AnimateClick(targetButton);
        }
    }

    private async void PasteButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var content = Clipboard.GetContent();
            if (content != null && content.Contains(StandardDataFormats.Text))
            {
                string pasteText = await content.GetTextAsync();
                ViewModel.PasteCommand.Execute(pasteText);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Paste Error: {ex.Message}");
        }
    }

    private void AdvancedCalculatorPage_Loaded(object sender, RoutedEventArgs e)
    {
        this.Focus(FocusState.Programmatic);
    }

    private async void CopyButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(DisplayTextBox.Text)) return;
        var dp = new DataPackage();
        dp.SetText(DisplayTextBox.Text);
        Clipboard.SetContent(dp);
        CopyNotification.IsOpen = true;
        await Task.Delay(2000);
        CopyNotification.IsOpen = false;
    }

    private async void OpenHistoryDialog()
    {
        if (_isDialogOpen) return;
        _isDialogOpen = true;

        try
        {
            var historyPage = new HistoryPage();
            historyPage.SetHistory(ViewModel.CalculationHistory.ToList());
            historyPage.HorizontalAlignment = HorizontalAlignment.Stretch;
            historyPage.VerticalAlignment = VerticalAlignment.Stretch;
            historyPage.HistoryItemSelected += (result) =>
            {
                ViewModel.DisplayText = result;
                ViewModel.CursorPosition = result.Length;
            };
            var dialog = new ContentDialog
            {
                Content = historyPage,
                CloseButtonText = "Close",
                XamlRoot = this.XamlRoot,
                RequestedTheme = this.RequestedTheme,
                MaxWidth = 600,
                Padding = new Thickness(20),
            };

            await dialog.ShowAsync();
        }
        finally
        {
            _isDialogOpen = false;
        }
    }

    private void HistoryButton_Click(object sender, RoutedEventArgs e) => OpenHistoryDialog();

    private async void ClearHistoryButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.CalculationHistory.Clear();
        UpdateHistorySidebar();
        
        // Delete the history file
        try
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var historyFile = await localFolder.TryGetItemAsync("advanced_calculator_history.json");
            if (historyFile != null)
            {
                await historyFile.DeleteAsync();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error deleting history file: {ex.Message}");
        }
    }

    private void UseHistorySidebarButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string historyItem)
        {
            string[] parts = historyItem.Split(new[] { " = " }, StringSplitOptions.None);
            if (parts.Length >= 2)
            {
                string result = parts[1];
                ViewModel.DisplayText = result;
                ViewModel.CursorPosition = result.Length;
            }
        }
    }
    private void DisplayTextBox_TextChanged(
    object sender,
    TextChangedEventArgs e)
    {
        ResizeDisplayText();
    }

    private void DisplayTextBox_SizeChanged(
        object sender,
        SizeChangedEventArgs e)
    {
        ResizeDisplayText();
    }

    private void ResizeDisplayText()
    {
        if (DisplayTextBox == null)
            return;

        string text = DisplayTextBox.Text ?? string.Empty;

        // Always restore the maximum size first.
        DisplayTextBox.FontSize = MaxDisplayFontSize;

        if (string.IsNullOrEmpty(text))
            return;

        double availableWidth =
            DisplayTextBox.ActualWidth
            - DisplayTextBox.Padding.Left
            - DisplayTextBox.Padding.Right;

        if (availableWidth <= 0)
            return;

        // Measure the actual width of the text.
        DisplayTextBox.Measure(
            new Windows.Foundation.Size(
                double.PositiveInfinity,
                double.PositiveInfinity));

        double textWidth = DisplayTextBox.DesiredSize.Width;

        if (textWidth <= availableWidth)
            return;

        // Shrink proportionally.
        double newFontSize =
            MaxDisplayFontSize * availableWidth / textWidth;

        DisplayTextBox.FontSize = Math.Max(
            MinDisplayFontSize,
            newFontSize);
    }
}