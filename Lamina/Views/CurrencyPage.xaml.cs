using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Lamina.Views;

public sealed partial class CurrencyPage : Page
{
    private readonly string _apiKey;
    private readonly HttpClient _httpClient = new();

    public CurrencyPage()
    {
        InitializeComponent();
        // Read API key from configuration
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
        _apiKey = config["CurrencyApiKey"] ?? string.Empty;
        _ = LoadCurrenciesAsync();
    }

    private class CurrencyItem
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public override string ToString() => $"{Code} - {Name}";
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
            var currencyList = new List<CurrencyItem>();
            foreach (var code in codes.EnumerateArray())
            {
                currencyList.Add(new CurrencyItem
                {
                    Code = code[0].GetString(),
                    Name = code[1].GetString()
                });
            }
            currencyList.Sort((a, b) => string.Compare(a.Code, b.Code, StringComparison.Ordinal));
            FromCurrencyComboBox.ItemsSource = currencyList;
            ToCurrencyComboBox.ItemsSource = currencyList;
            FromCurrencyComboBox.SelectedItem = currencyList.Find(c => c.Code == "USD");
            ToCurrencyComboBox.SelectedItem = currencyList.Find(c => c.Code == "EUR");
        }
        catch
        {
            // Fallback: major world currencies with names
            var fallback = new List<CurrencyItem>
            {
                new CurrencyItem { Code = "USD", Name = "United States Dollar" },
                new CurrencyItem { Code = "EUR", Name = "Euro" },
                new CurrencyItem { Code = "GBP", Name = "British Pound Sterling" },
                new CurrencyItem { Code = "INR", Name = "Indian Rupee" },
                new CurrencyItem { Code = "JPY", Name = "Japanese Yen" },
                new CurrencyItem { Code = "CNY", Name = "Chinese Yuan" },
                new CurrencyItem { Code = "AUD", Name = "Australian Dollar" },
                new CurrencyItem { Code = "CAD", Name = "Canadian Dollar" },
                new CurrencyItem { Code = "CHF", Name = "Swiss Franc" },
                new CurrencyItem { Code = "SGD", Name = "Singapore Dollar" },
                new CurrencyItem { Code = "ZAR", Name = "South African Rand" },
                new CurrencyItem { Code = "BRL", Name = "Brazilian Real" },
                new CurrencyItem { Code = "RUB", Name = "Russian Ruble" },
                new CurrencyItem { Code = "KRW", Name = "South Korean Won" },
                new CurrencyItem { Code = "MXN", Name = "Mexican Peso" },
                new CurrencyItem { Code = "HKD", Name = "Hong Kong Dollar" },
                new CurrencyItem { Code = "NZD", Name = "New Zealand Dollar" },
                new CurrencyItem { Code = "SEK", Name = "Swedish Krona" },
                new CurrencyItem { Code = "TRY", Name = "Turkish Lira" },
                new CurrencyItem { Code = "SAR", Name = "Saudi Riyal" }
            };
            FromCurrencyComboBox.ItemsSource = fallback;
            ToCurrencyComboBox.ItemsSource = fallback;
            FromCurrencyComboBox.SelectedItem = fallback.Find(c => c.Code == "USD");
            ToCurrencyComboBox.SelectedItem = fallback.Find(c => c.Code == "EUR");
        }
        finally
        {
            LoadingProgressBar.Visibility = Visibility.Collapsed;
        }
    }

    private void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        _ = LoadCurrenciesAsync();
        ResultTextBlock.Text = "Currency list refreshed.";
    }

    private async void ConvertButton_Click(object sender, RoutedEventArgs e)
    {
        if (decimal.TryParse(InputTextBox.Text, out decimal amount))
        {
            var fromItem = FromCurrencyComboBox.SelectedItem as CurrencyItem;
            var toItem = ToCurrencyComboBox.SelectedItem as CurrencyItem;
            var from = fromItem?.Code ?? "USD";
            var to = toItem?.Code ?? "USD";
            try
            {
                string url = $"https://v6.exchangerate-api.com/v6/{_apiKey}/pair/{from}/{to}/{amount}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var result = doc.RootElement.GetProperty("conversion_result").GetDecimal();
                ResultTextBlock.Text = $"{amount} {from} = {result} {to}";
            }
            catch (Exception ex)
            {
                ResultTextBlock.Text = $"Error: {ex.Message}";
            }
        }
        else
        {
            ResultTextBlock.Text = "Invalid amount.";
        }
    }
}