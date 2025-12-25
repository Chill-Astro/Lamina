using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Lamina.Views;

public sealed partial class DiagonalPage : Page
{
    public DiagonalPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        // Load the Square diagonal page by default
        ContentFrame.Navigate(typeof(SquareDiagPage));
    }

    private void ShapeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ShapeComboBox.SelectedItem is ComboBoxItem selectedItem)
        {
            string shape = selectedItem.Content.ToString();

            if (ContentFrame == null)
            {
                System.Diagnostics.Debug.WriteLine("ContentFrame is null.");
                return;
            }

            switch (shape)
            {
                case "Square":
                    ContentFrame.Navigate(typeof(SquareDiagPage));
                    break;
                case "Rectangle":
                    ContentFrame.Navigate(typeof(RectDiagPage));
                    break;
                case "Cube":
                    ContentFrame.Navigate(typeof(CubeDiagPage));
                    break;
                case "Cuboid":
                    ContentFrame.Navigate(typeof(CuboidDiagPage));
                    break;
            }
        }
    }
}