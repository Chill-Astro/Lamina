using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Lamina.Views;

public sealed partial class PerimeterPage : Page
{
    public PerimeterPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        // Load the Equilateral Triangle perimeter page by default
        ContentFrame.Navigate(typeof(ETPermPage));
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
                case "Equilateral Triangle":
                    ContentFrame.Navigate(typeof(ETPermPage));
                    break;
                case "Isosceles Triangle":
                    ContentFrame.Navigate(typeof(ITPermPage));
                    break;
                case "Square / Rhombus":
                    ContentFrame.Navigate(typeof(SquarePermPage));
                    break;
                case "Rectangle / Parallelogram":
                    ContentFrame.Navigate(typeof(RectPermPage));
                    break;
                case "Circle":
                    ContentFrame.Navigate(typeof(CirclePermPage));
                    break;
                case "Semi-circle":
                    ContentFrame.Navigate(typeof(SCirclePermPage));
                    break;
            }
        }
    }
}