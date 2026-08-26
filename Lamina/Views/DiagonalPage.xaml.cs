using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.ApplicationModel.DataTransfer;
namespace Lamina.Views
{
    public sealed partial class DiagonalPage : Page
    {
        static string temp = "";

        public DiagonalPage()
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
                case "Square":
                    FormulaInfoBar.Message = "d = a√2";
                    SetInputs("Side (a)");
                    break;
                case "Rectangle":
                    FormulaInfoBar.Message = "d = √(L² + W²)";
                    SetInputs("Length (L)", "Width (W)");
                    break;
                case "Cube":
                    FormulaInfoBar.Message = "d = a√3";
                    SetInputs("Side (a)");
                    break;
                case "Cuboid":
                    FormulaInfoBar.Message = "d = √(l² + w² + h²)";
                    SetInputs("Length (l)", "Width (w)", "Height (h)");
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
            double diagonal = 0;

            if (double.IsNaN(a) ||
               (InputB.Visibility == Visibility.Visible && double.IsNaN(b)) ||
               (InputC.Visibility == Visibility.Visible && double.IsNaN(c)))
            {
                temp = "";
                await ShowResultPopup("Error:", "Invalid inputs.", false);
                return;
            }

            try
            {
                // MATH TIME (Don't worry, it's just applying the formulae)
                switch (shape)
                {
                    case "Square": diagonal = a * Math.Sqrt(2); break;
                    case "Rectangle": diagonal = Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2)); break;
                    case "Cube": diagonal = a * Math.Sqrt(3); break;
                    case "Cuboid": diagonal = Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2) + Math.Pow(c, 2)); break;
                }

                temp = diagonal.ToString("N2");
                await ShowResultPopup("Diagonal Length =", temp, true);
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