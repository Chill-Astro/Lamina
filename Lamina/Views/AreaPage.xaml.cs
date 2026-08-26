using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.ApplicationModel.DataTransfer;
namespace Lamina.Views
{
    public sealed partial class AreaPage : Page
    {
        static string temp = "";

        public AreaPage()
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
            if (string.IsNullOrEmpty(shape)) return;

            switch (shape)
            {
                // Formulae
                case "Equilateral Triangle":
                    FormulaInfoBar.Message = "Area = (√3 / 4) × s²";
                    SetInputs("Side (s)");
                    break;
                case "Isosceles Triangle":
                case "Standard Triangle":
                    FormulaInfoBar.Message = "Area = ½ × b × h";
                    SetInputs("Base (b)", "Height (h)");
                    break;
                case "Square":
                    FormulaInfoBar.Message = "Area = s²";
                    SetInputs("Side (s)");
                    break;
                case "Rectangle / Parallelogram":
                    FormulaInfoBar.Message = "Area = L × W";
                    SetInputs("Length (L)", "Width (W)");
                    break;
                case "Rhombus":
                    FormulaInfoBar.Message = "Area = (d1 × d2) / 2";
                    SetInputs("Diagonal 1 (d1)", "Diagonal 2 (d2)");
                    break;
                case "Circle":
                    FormulaInfoBar.Message = "Area = πr²";
                    SetInputs("Radius (r)");
                    break;
                case "Semi-circle":
                    FormulaInfoBar.Message = "Area = (πr²) / 2";
                    SetInputs("Radius (r)");
                    break;
                case "Room (4 Walls)":
                    FormulaInfoBar.Message = "Area = 2h(L + W)";
                    SetInputs("Length (L)", "Width (W)", "Height (h)");
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
            double area = 0;

            if (double.IsNaN(a) ||
               (InputB.Visibility == Visibility.Visible && double.IsNaN(b)) ||
               (InputC.Visibility == Visibility.Visible && double.IsNaN(c)))
            {
                temp = "";
                await ShowResultPopup("Error:", "Please Enter Valid Numbers", false);
                return;
            }

            try
            {
                // MATH TIME! (Don't worry, it's just basic math)
                switch (shape)
                {
                    case "Equilateral Triangle": area = (Math.Sqrt(3) / 4) * Math.Pow(a, 2); break;
                    case "Isosceles Triangle":
                    case "Standard Triangle": area = 0.5 * a * b; break;
                    case "Square": area = Math.Pow(a, 2); break;
                    case "Rectangle / Parallelogram": area = a * b; break;
                    case "Rhombus": area = (a * b) / 2; break;
                    case "Circle": area = Math.PI * Math.Pow(a, 2); break;
                    case "Semi-circle": area = (Math.PI * Math.Pow(a, 2)) / 2; break;
                    case "Room (4 Walls)": area = 2 * c * (a + b); break;
                }

                if (double.IsNaN(area) || double.IsInfinity(area)) throw new Exception();

                temp = area.ToString("N2");
                await ShowResultPopup("Area =", temp, true);
            }
            catch
            {
                temp = "";
                await ShowResultPopup("Error:", "Invalid Dimensions", false);
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