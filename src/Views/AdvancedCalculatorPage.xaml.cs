using System;
using System.Linq;
using System.Threading.Tasks;
using Lamina.ViewModels;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using Windows.UI.Core;

namespace Lamina.Views;

public sealed partial class AdvancedCalculatorPage : Page
{
    private bool _isDialogOpen;
    public AdvancedCalculatorViewModel ViewModel { get; }

    public AdvancedCalculatorPage()
    {
        ViewModel = App.GetService<AdvancedCalculatorViewModel>();
        InitializeComponent();
        this.Loaded += (s, e) => this.Focus(FocusState.Programmatic);
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
                    ViewModel.SetOperatorCommand.Execute("^");
                    targetButton = BtnPow;
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
                case VirtualKey.Comma:
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
        if (string.IsNullOrEmpty(DisplayTextBlock.Text)) return;
        var dp = new DataPackage();
        dp.SetText(DisplayTextBlock.Text);
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
            historyPage.HorizontalAlignment = HorizontalAlignment.Center;
            historyPage.VerticalAlignment = VerticalAlignment.Center;
            var dialog = new ContentDialog
            {
                Content = historyPage,
                CloseButtonText = "Close",
                XamlRoot = this.XamlRoot,
                RequestedTheme = this.RequestedTheme,
                MaxWidth = 400,
            };

            await dialog.ShowAsync();
        }
        finally
        {
            _isDialogOpen = false;
        }
    }

    private void HistoryButton_Click(object sender, RoutedEventArgs e) => OpenHistoryDialog();
}