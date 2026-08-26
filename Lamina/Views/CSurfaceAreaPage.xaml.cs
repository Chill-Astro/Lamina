using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.ApplicationModel.DataTransfer;
namespace Lamina.Views
{
    public sealed partial class CSurfaceAreaPage : Page
    {
        static string temp = "";

        public CSurfaceAreaPage()
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
                case "Cylinder":
                    FormulaInfoBar.Message = "CSA = 2πrh";
                    SetInputs("Radius (r)", "Height (h)");
                    break;
                case "Cone":
                    FormulaInfoBar.Message = "CSA = πrl";
                    SetInputs("Radius (r)", "Slant Height (l)");
                    break;
                case "Sphere":
                    FormulaInfoBar.Message = "CSA = 4πr²";
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
            double csa = 0;

            if (double.IsNaN(a) || (InputB.Visibility == Visibility.Visible && double.IsNaN(b)))
            {
                temp = "";
                await ShowResultPopup("Error:", "Invalid inputs.", false);
                return;
            }

            try
            {
                // MATH TIME! (Don't worry, it's just basic geometry formulas)
                switch (shape)
                {
                    case "Cylinder": csa = 2 * Math.PI * a * b; break;
                    case "Cone": csa = Math.PI * a * b; break;
                    case "Sphere": csa = 4 * Math.PI * Math.Pow(a, 2); break;
                }

                if (double.IsNaN(csa) || double.IsInfinity(csa)) throw new Exception();

                temp = csa.ToString("N2");
                await ShowResultPopup("C. Surface Area =", temp, true);
            }
            catch
            {
                temp = "";
                await ShowResultPopup("Error:", "Calculation error.", false);
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