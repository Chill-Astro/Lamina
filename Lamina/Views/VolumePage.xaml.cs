using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.ApplicationModel.DataTransfer;
namespace Lamina.Views
{
    public sealed partial class VolumePage : Page
    {
        static string temp = "";

        public VolumePage()
        {
            this.InitializeComponent();
            this.Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ShapeComboBox.SelectedIndex = 0;
            UpdateUI();
        }

        private void ShapeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (ShapeComboBox == null || InputA == null || FormulaInfoBar == null) return;

            InputA.Visibility = InputB.Visibility = InputC.Visibility = Visibility.Collapsed;
            string shape = (ShapeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();

            switch (shape)
            {
                // Formulae
                case "Cube":
                    FormulaInfoBar.Message = "V = a³";
                    SetInputs("Side (a)");
                    break;
                case "Cuboid":
                    FormulaInfoBar.Message = "V = l × w × h";
                    SetInputs("Length (l)", "Width (w)", "Height (h)");
                    break;
                case "Cylinder":
                    FormulaInfoBar.Message = "V = πr²h";
                    SetInputs("Radius (r)", "Height (h)");
                    break;
                case "Cone":
                    FormulaInfoBar.Message = "V = (1/3)πr²h";
                    SetInputs("Radius (r)", "Height (h)");
                    break;
                case "Sphere":
                    FormulaInfoBar.Message = "V = (4/3)πr³";
                    SetInputs("Radius (r)");
                    break;
            }
        }

        private void SetInputs(string a, string b = null, string c = null)
        {
            InputA.Header = a; InputA.Visibility = Visibility.Visible;
            if (b != null) { InputB.Header = b; InputB.Visibility = Visibility.Visible; }
            if (c != null) { InputC.Header = c; InputC.Visibility = Visibility.Visible; }
        }

        private async void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            string shape = (ShapeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            double a = InputA.Value;
            double b = InputB.Value;
            double c = InputC.Value;
            double volume = 0;

            if (double.IsNaN(a) ||
               (InputB.Visibility == Visibility.Visible && double.IsNaN(b)) ||
               (InputC.Visibility == Visibility.Visible && double.IsNaN(c)))
            {
                temp = "";
                await ShowResultPopup("Error :", "Invalid Inputs", false);
                return;
            }

            try
            {
                switch (shape)
                {
                    // MATH TIME! (Don't worry, it's just basic math)
                    case "Cube": volume = Math.Pow(a, 3); break;
                    case "Cuboid": volume = a * b * c; break;
                    case "Cylinder": volume = Math.PI * Math.Pow(a, 2) * b; break;
                    case "Cone": volume = (Math.PI * Math.Pow(a, 2) * b) / 3.0; break;
                    case "Sphere": volume = (4.0 / 3.0) * Math.PI * Math.Pow(a, 3); break;
                }

                if (double.IsNaN(volume) || double.IsInfinity(volume)) throw new Exception();

                temp = volume.ToString("N2");
                await ShowResultPopup("Volume =", temp, true);
            }
            catch
            {
                temp = "";
                await ShowResultPopup("Error :", "Calculation Error", false);
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