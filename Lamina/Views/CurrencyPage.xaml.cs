using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Lamina.Views;

public sealed partial class CurrencyPage : Page
{
    private readonly string _apiKey;
    private readonly HttpClient _httpClient = new();

    public CurrencyPage()
    {
        InitializeComponent();
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) // Getting API key from appsettings.json to not leak my Personal API Key!
            .Build();
        _apiKey = config["CurrencyApiKey"] ?? string.Empty;
        _ = LoadCurrenciesAsync();
    }

    private class CurrencyItem
    {
        public string Code { get; set; }
        public string Name { get; set; }        
        public override string ToString() => Code;
    }

    private async Task LoadCurrenciesAsync()
    {
        LoadingProgressBar.Visibility = Visibility.Visible;
        try
        {
            string url = $"https://v6.exchangerate-api.com/v6/{_apiKey}/codes";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var codes = doc.RootElement.GetProperty("supported_codes");

            var currencyList = codes.EnumerateArray()
                .Select(c => new CurrencyItem { Code = c[0].GetString(), Name = c[1].GetString() })
                .OrderBy(c => c.Code)
                .ToList();

            FromCurrency.ItemsSource = currencyList;
            ToCurrency.ItemsSource = currencyList;
            FromCurrency.SelectedItem = currencyList.Find(c => c.Code == "USD");
            ToCurrency.SelectedItem = currencyList.Find(c => c.Code == "EUR");
        }
        catch
        {
            var fallback = new List<CurrencyItem> { new() { Code = "USD", Name = "United States Dollar" }, new() { Code = "EUR", Name = "Euro" } };
            FromCurrency.ItemsSource = fallback;
            ToCurrency.ItemsSource = fallback;
        }
        finally
        {
            LoadingProgressBar.Visibility = Visibility.Collapsed;
        }
    }

    private void Refresh_Click(object sender, RoutedEventArgs e)
    {
        _ = LoadCurrenciesAsync();
    }

    private async void Convert_Click(object sender, RoutedEventArgs e)
    {
        double amount = InputNumberBox.Value;
        if (double.IsNaN(amount))
        {
            ResultTextBlock.Text = "Invalid Amount";
            return;
        }

        var from = (FromCurrency.SelectedItem as CurrencyItem)?.Code ?? "USD";
        var to = (ToCurrency.SelectedItem as CurrencyItem)?.Code ?? "EUR";

        LoadingProgressBar.Visibility = Visibility.Visible;
        try
        {
            string url = $"https://v6.exchangerate-api.com/v6/{_apiKey}/pair/{from}/{to}/{amount}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var result = doc.RootElement.GetProperty("conversion_result").GetDouble();

            // Restored original output style
            ResultTextBlock.Text = $"{result:N2} {to}";
        }
        catch (Exception ex)
        {            
        }
        finally
        {
            LoadingProgressBar.Visibility = Visibility.Collapsed;
        }
    }
    private void SwapButton_Click(object sender, RoutedEventArgs e)
    {
        var temp = FromCurrency.SelectedIndex;
        FromCurrency.SelectedIndex = ToCurrency.SelectedIndex;
        ToCurrency.SelectedIndex = temp;

        // Auto-convert on swap if there is a value
        if (!double.IsNaN(InputNumberBox.Value))
        {
            Convert_Click(null, null);
        }
    }
    private void CopyButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(ResultTextBlock.Text) || ResultTextBlock.Text == "---") return;

        var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
        dataPackage.SetText(ResultTextBlock.Text);
        Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);        
        VisualStateManager.GoToState(CopyButton, "Normal", true);
    }
}