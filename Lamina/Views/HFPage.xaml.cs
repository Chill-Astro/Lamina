using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.ApplicationModel.DataTransfer;
namespace Lamina.Views
{
    public sealed partial class HFPage : Page
    {
        static string temp = "";

        public HFPage()
        {
            this.InitializeComponent();
        }

        private async void Calculate_Click(object sender, RoutedEventArgs e)
        {
            double a = SideA.Value;
            double b = SideB.Value;
            double c = SideC.Value;

            // Validation for Missing / Zero / Negative ( Basically not Joking with Shapes )
            if (double.IsNaN(a) || double.IsNaN(b) || double.IsNaN(c) || a <= 0 || b <= 0 || c <= 0)
            {
                temp = "";
                await ShowResultPopup("Error :", "Invalid Side Lengths", false);
                return;
            }

            // Triangle Inequality Check ( Sum of any two sides must be greater than the third )
            if (a + b <= c || a + c <= b || b + c <= a)
            {
                temp = "";
                await ShowResultPopup("Invalid Geometry :", "Invalid Triangle", false);
                return;
            }

            // MATHING TIME!
            try
            {
                double s = (a + b + c) / 2.0;
                double area = Math.Sqrt(s * (s - a) * (s - b) * (s - c));

                if (double.IsNaN(area) || double.IsInfinity(area)) throw new Exception();

                temp = area.ToString("N2");
                await ShowResultPopup("Area =", temp, true);
            }
            catch
            {
                temp = "";
                await ShowResultPopup("Error :", "Calculation Failed", false);
            }
        }

        private async System.Threading.Tasks.Task ShowResultPopup(string contextText, string actualValue, bool isSuccess)
        {
            ResultLabel.Text = contextText;
            ResultValueText.Text = actualValue;

            // Visuals
            CopyButton.Visibility = isSuccess ? Visibility.Visible : Visibility.Collapsed;

            // Red has highest Wavelength so it's the color chosen for errors. ( I hope you remember Physics :D )
            ResultValueText.Foreground = isSuccess
                ? (Brush)Application.Current.Resources["TextFillColorPrimaryBrush"]
                : (Brush)Application.Current.Resources["SystemFillColorCriticalBrush"];

            ResultDialog.XamlRoot = this.Content.XamlRoot;
            await ResultDialog.ShowAsync();
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(temp)) return;
            var dataPackage = new DataPackage();
            dataPackage.SetText(temp);
            Clipboard.SetContent(dataPackage);
        }
    }
}