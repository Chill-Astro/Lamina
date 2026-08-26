using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using Windows.Storage;

namespace Lamina.Views
{
    // Just like School History, this too is Boring.
    public sealed partial class HistoryPage : Page
    {
        private List<string> _history;
        public event Action<string> HistoryItemSelected;

        public HistoryPage()
        {
            this.InitializeComponent();
            _history = new List<string>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is List<string> history)
            {
                SetHistory(history);
            }
        }

        private async void ClearHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear the history list
            _history.Clear();
            UpdateHistoryDisplay();

            // Also delete the history files
            try
            {
                var localFolder = ApplicationData.Current.LocalFolder;
                var historyFile = await localFolder.TryGetItemAsync("calculator_history.json");
                if (historyFile != null)
                {
                    await historyFile.DeleteAsync();
                }

                var advancedHistoryFile = await localFolder.TryGetItemAsync("advanced_calculator_history.json");
                if (advancedHistoryFile != null)
                {
                    await advancedHistoryFile.DeleteAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting history files: {ex.Message}");
            }
        }

        public void SetHistory(List<string> history)
        {
            _history = history ?? new List<string>();
            UpdateHistoryDisplay();
        }

        private void UpdateHistoryDisplay()
        {
            if (_history == null || _history.Count == 0)
            {
                NoHistoryText.Visibility = Visibility.Visible;
                HistoryList.ItemsSource = null;
            }
            else
            {
                NoHistoryText.Visibility = Visibility.Collapsed;
                HistoryList.ItemsSource = null;
                HistoryList.ItemsSource = _history;
            }
        }

        private void UseHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string historyItem)
            {
                string result = historyItem;

                // Safely split by '=' regardless of surrounding spaces
                if (historyItem.Contains('='))
                {
                    var parts = historyItem.Split('=');
                    if (parts.Length >= 2)
                    {
                        // Take the last part as the result (e.g., "10" from "5 + 5 = 10")
                        result = parts[parts.Length - 1].Trim();
                    }
                }

                HistoryItemSelected?.Invoke(result);
            }
        }
    }
}