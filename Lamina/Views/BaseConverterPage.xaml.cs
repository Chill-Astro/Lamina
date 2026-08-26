using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.ApplicationModel.DataTransfer;

namespace Lamina.Views;

public sealed partial class BaseConverterPage : Page
{
    static string temp = "";
    public BaseConverterPage() => this.InitializeComponent();

    private async void Convert_Click(object sender, RoutedEventArgs e)
    {
        string input = Input.Text?.Trim().ToUpper();

        if (string.IsNullOrEmpty(input))
        {
            await ShowResultPopup("Input Issue :", "Please Enter a Number", false);
            return;
        }

        try
        {
            int fromBase = int.Parse((FromBase.SelectedItem as ComboBoxItem).Tag.ToString());
            int toBase = int.Parse((ToBase.SelectedItem as ComboBoxItem).Tag.ToString());

            // Convert input to decimal first, then to target base
            long decimalValue = System.Convert.ToInt64(input, fromBase);
            temp = System.Convert.ToString(decimalValue, toBase).ToUpper();

            string toBaseName = (ToBase.SelectedItem as ComboBoxItem).Content.ToString().Split(' ')[0];
            await ShowResultPopup($"{toBaseName} Value =", temp, true);
        }
        catch (FormatException)
        {
            await ShowResultPopup("Issue :", "Invalid Digits for Selected Base ", false);
        }
        catch (OverflowException)
        {
            await ShowResultPopup("Issue :", "Number too Large", false);
        }
        catch (Exception)
        {
            await ShowResultPopup("Issue :", "Conversion Failed", false);
        }
    }

    private async System.Threading.Tasks.Task ShowResultPopup(string label, string value, bool isSuccess)
    {
        ResultLabel.Text = label;
        ResultValueText.Text = value;

        // Only show copy button on Success, as Success stands on the pillars of Failure.
        CopyButton.Visibility = isSuccess ? Visibility.Visible : Visibility.Collapsed;

        // Red has highest Wavelength so it's the color chosen for errors. ( I hope you remember Physics :D )
        ResultValueText.Foreground = isSuccess
            ? (Brush)Application.Current.Resources["TextFillColorPrimaryBrush"]
            : (Brush)Application.Current.Resources["SystemFillColorCriticalBrush"]; // This makes it red

        ResultDialog.XamlRoot = this.Content.XamlRoot;
        await ResultDialog.ShowAsync();
    }

    private void CopyButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(temp)) return;
        var dp = new DataPackage();
        dp.SetText(temp);
        Clipboard.SetContent(dp);
    }
}