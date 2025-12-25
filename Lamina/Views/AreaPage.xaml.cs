using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation; // Add this using statement

namespace Lamina.Views;

public sealed partial class AreaPage : Page
{
    public AreaPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        // Load the Equilateral Triangle page by default
        ContentFrame.Navigate(typeof(ETAreaPage));
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
                    ContentFrame.Navigate(typeof(ETAreaPage));
                    break;
                case "Isosceles Triangle":
                    ContentFrame.Navigate(typeof(ITAreaPage));
                    break;
                case "Standard Triangle":
                    ContentFrame.Navigate(typeof(TriAreaPage));
                    break;
                case "Square":
                    ContentFrame.Navigate(typeof(SquareAreaPage));
                    break;
                case "Rectangle / Parallelogram":
                    ContentFrame.Navigate(typeof(RectAreaPage));
                    break;
                case "Rhombus":
                    ContentFrame.Navigate(typeof(RhombusAreaPage));
                    break;
                case "Circle":
                    ContentFrame.Navigate(typeof(CircleAreaPage));
                    break;
                case "Semi-circle":
                    ContentFrame.Navigate(typeof(SCircleAreaPage));
                    break;
                case "Room":
                    ContentFrame.Navigate(typeof(RoomAreaPage));
                    break;
            }
        }
    }
}