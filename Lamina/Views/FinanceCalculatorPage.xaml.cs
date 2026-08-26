using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.ApplicationModel.DataTransfer;
namespace Lamina.Views
{
    public sealed partial class FinanceCalculatorPage : Page
    {
        static string temp = "";

        public FinanceCalculatorPage()
        {
            this.InitializeComponent();
            this.Loaded += (s, e) => { ComboBox.SelectedIndex = 0; UpdateUI(); };
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateUI();

        private void UpdateUI()
        {
            if (ComboBox == null || InputA == null) return;

            InputA.Visibility = InputB.Visibility = InputC.Visibility = InputD.Visibility = Visibility.Collapsed;
            string type = (ComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();

            switch (type)
            {
                // Formulae
                case "Simple Interest":
                    FormulaInfoBar.Message = "I = (P × R × T) / 100";
                    SetInputs("Principal (P)", "Rate % (R)", "Years (T)");
                    break;
                case "Compound Interest":
                    FormulaInfoBar.Message = "A = P(1 + r/n)^nt";
                    SetInputs("Principal (P)", "Rate % (r)", "Years (t)", "Compounding periods per year (n)");
                    InputD.Value = 12;
                    break;
                case "Recurring Deposit":
                    FormulaInfoBar.Message = "I = [P × n(n+1) / 24] × r/100";
                    SetInputs("Monthly Deposit (P)", "Rate % (r)", "Total Months (n)");
                    break;
            }
        }

        private void SetInputs(string a, string b, string c, string d = null)
        {
            InputA.Header = a; InputA.Visibility = Visibility.Visible;
            InputB.Header = b; InputB.Visibility = Visibility.Visible;
            InputC.Header = c; InputC.Visibility = Visibility.Visible;
            if (d != null) { InputD.Header = d; InputD.Visibility = Visibility.Visible; }
        }

        private async void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            string type = (ComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            double p = InputA.Value;
            double r = InputB.Value;
            double t = InputC.Value;
            double n = InputD.Value;

            double interest = 0, totalAmount = 0;

            if (double.IsNaN(p) || double.IsNaN(r) || double.IsNaN(t))
            {
                temp = "";
                await ShowResultPopup("Error :", "Please Fill All Fields", false);
                return;
            }

            try
            {
                switch (type)
                {
                    // MATH TIME (I hope you remember all these formulae from school :D )
                    case "Simple Interest":
                        interest = (p * r * t) / 100.0;
                        totalAmount = p + interest;
                        temp = interest.ToString("N2");
                        await ShowResultPopup($"Interest = {totalAmount:N2}", temp, true);
                        break;

                    case "Compound Interest":
                        if (double.IsNaN(n) || n <= 0) n = 1;
                        totalAmount = p * Math.Pow((1 + (r / 100.0) / n), n * t);
                        interest = totalAmount - p;
                        temp = totalAmount.ToString("N2");
                        await ShowResultPopup($"Compound Interest = {interest:N2}", temp, true);
                        break;

                    case "Recurring Deposit":
                        interest = (p * t * (t + 1) / 24.0) * (r / 100.0);
                        totalAmount = (p * t) + interest;
                        temp = interest.ToString("N2");
                        await ShowResultPopup($"Maturity Value = {totalAmount:N2}", temp, true);
                        break;
                }
            }
            catch
            {
                temp = "";
                await ShowResultPopup("Error :", "Calculation failed", false);
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