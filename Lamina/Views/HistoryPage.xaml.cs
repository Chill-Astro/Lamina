using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Generic;

namespace Lamina.Views
{
    public sealed partial class HistoryPage : Page
    {
        private List<string> _history;

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

        private void ClearHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear the history list
            _history.Clear();
            UpdateHistoryDisplay();
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
                NoHistoryTextBlock.Visibility = Visibility.Visible;
                HistoryListView.ItemsSource = null;
            }
            else
            {
                NoHistoryTextBlock.Visibility = Visibility.Collapsed;
                HistoryListView.ItemsSource = _history;
            }
        }
    }
}
