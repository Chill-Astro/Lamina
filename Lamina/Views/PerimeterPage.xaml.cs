using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.ApplicationModel.DataTransfer;
namespace Lamina.Views
{
    public sealed partial class PerimeterPage : Page
    {
        static string temp = "";

        public PerimeterPage()
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

            InputA.Visibility = InputB.Visibility = Visibility.Collapsed;
            string shape = (ShapeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();

            switch (shape)
            {
                // Formulae
                case "Equilateral Triangle":
                    FormulaInfoBar.Message = "Perimeter = 3 × s";
                    SetInputs("Side (s)");
                    break;
                case "Isosceles Triangle":
                    FormulaInfoBar.Message = "Perimeter = 2a + b";
                    SetInputs("Equal Side (a)", "Base (b)");
                    break;
                case "Square / Rhombus":
                    FormulaInfoBar.Message = "Perimeter = 4 × s";
                    SetInputs("Side (s)");
                    break;
                case "Rectangle / Parallelogram":
                    FormulaInfoBar.Message = "Perimeter = 2 × (L + W)";
                    SetInputs("Length (L)", "Width (W)");
                    break;
                case "Circle":
                    FormulaInfoBar.Message = "Circumference = 2πr";
                    SetInputs("Radius (r)");
                    break;
                case "Semi-circle":
                    FormulaInfoBar.Message = "Perimeter = πr + 2r";
                    SetInputs("Radius (r)");
                    break;
            }
        }

        private void SetInputs(string a, string b = null)
        {
            InputA.Header = a; InputA.Visibility = Visibility.Visible;
            if (b != null) { InputB.Header = b; InputB.Visibility = Visibility.Visible; }
        }

        private async void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            string shape = (ShapeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            double a = InputA.Value;
            double b = InputB.Value;
            double perimeter = 0;

            if (double.IsNaN(a) || (InputB.Visibility == Visibility.Visible && double.IsNaN(b)))
            {
                temp = "";
                await ShowResultPopup("Error :", "Please Enter Valid Numbers", false);
                return;
            }

            try
            {
                switch (shape)
                {
                    // MATH TIME
                    case "Equilateral Triangle": perimeter = 3 * a; break;
                    case "Isosceles Triangle": perimeter = (2 * a) + b; break;
                    case "Square / Rhombus": perimeter = 4 * a; break;
                    case "Rectangle / Parallelogram": perimeter = 2 * (a + b); break;
                    case "Circle": perimeter = 2 * Math.PI * a; break;
                    case "Semi-circle": perimeter = (Math.PI * a) + (2 * a); break;
                }

                if (double.IsNaN(perimeter) || double.IsInfinity(perimeter)) throw new Exception();

                temp = perimeter.ToString("N2");
                await ShowResultPopup("Perimeter =", temp, true);
            }
            catch
            {
                temp = "";
                await ShowResultPopup("Error :", "Invalid Dimensions", false);
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