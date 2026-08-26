using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.ApplicationModel.DataTransfer;
namespace Lamina.Views
{
    public sealed partial class SurfaceAreaPage : Page
    {
        static string temp = "";

        public SurfaceAreaPage()
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
                    FormulaInfoBar.Message = "SA = 6a²";
                    SetInputs("Side (a)");
                    break;
                case "Cuboid":
                    FormulaInfoBar.Message = "SA = 2(lw + wh + hl)";
                    SetInputs("Length (l)", "Width (w)", "Height (h)");
                    break;
                case "Cylinder":
                    FormulaInfoBar.Message = "SA = 2πr(r + h)";
                    SetInputs("Radius (r)", "Height (h)");
                    break;
                case "Cone":
                    FormulaInfoBar.Message = "SA = πr(r + l)";
                    SetInputs("Radius (r)", "Slant Height (l)");
                    break;
                case "Sphere":
                    FormulaInfoBar.Message = "SA = 4πr²";
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
            double surfaceArea = 0;

            if (double.IsNaN(a) ||
               (InputB.Visibility == Visibility.Visible && double.IsNaN(b)) ||
               (InputC.Visibility == Visibility.Visible && double.IsNaN(c)))
            {
                temp = "";
                await ShowResultPopup("Error :", "Please Enter Valid Numbers", false);
                return;
            }

            try
            {
                switch (shape)
                {
                    // MATH TIME (Don't worry, I got this)
                    case "Cube":
                        surfaceArea = 6 * Math.Pow(a, 2);
                        break;
                    case "Cuboid":
                        // a=l, b=w, c=h
                        surfaceArea = 2 * (a * b + b * c + c * a);
                        break;
                    case "Cylinder":
                        // a=r, b=h
                        surfaceArea = 2 * Math.PI * a * (a + b);
                        break;
                    case "Cone":
                        // a=r, b=l (slant height)
                        surfaceArea = Math.PI * a * (a + b);
                        break;
                    case "Sphere":
                        surfaceArea = 4 * Math.PI * Math.Pow(a, 2);
                        break;
                }

                temp = surfaceArea.ToString("N2");
                await ShowResultPopup("Surface Area =", temp, true);
            }
            catch
            {
                temp = "";
                await ShowResultPopup("Error :", "Invalid dimensions", false);
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