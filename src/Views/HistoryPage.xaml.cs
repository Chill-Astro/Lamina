using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Linq;
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
            _history = history;
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
                HistoryList.ItemsSource = _history;
            }
        }

        private void UseHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string historyItem)
            {
                // Extract the result part (after " = ")
                string[] parts = historyItem.Split(new[] { " = " }, StringSplitOptions.None);
                if (parts.Length >= 2)
                {
                    string result = parts[1];
                    HistoryItemSelected?.Invoke(result);
                }
            }
        }

        private void HistoryItem_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (sender is Border border && border.Parent is Grid grid)
            {
                var button = grid.FindName("UseButton") as Button;
                if (button != null)
                {
                    button.Visibility = Visibility.Visible;
                }
            }
        }

        private void HistoryItem_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (sender is Border border && border.Parent is Grid grid)
            {
                var button = grid.FindName("UseButton") as Button;
                if (button != null)
                {
                    button.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
