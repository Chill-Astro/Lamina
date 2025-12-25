using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Lamina.Views;

public sealed partial class RectPermPage : Page
{
    public RectPermPage()
    {
        InitializeComponent();
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(LengthTextBox.Text, out double length) &&
            double.TryParse(WidthTextBox.Text, out double width))
        {
            double perimeter = 2 * (length + width);
            ResultTextBlock.Text = (perimeter.ToString("F2") + " units").ToUpper();
        }
        else
        {
            ResultTextBlock.Text = "INVALID INPUT".ToUpper();
        }
    }
}