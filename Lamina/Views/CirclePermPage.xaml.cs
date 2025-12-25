using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Lamina.Views;

public sealed partial class CirclePermPage : Page
{
    public CirclePermPage()
    {
        InitializeComponent();
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(RadiusTextBox.Text, out double radius))
        {
            double perimeter = 2 * Math.PI * radius;
            ResultTextBlock.Text = (perimeter.ToString("F2") + " units").ToUpper();
        }
        else
        {
            ResultTextBlock.Text = "INVALID INPUT".ToUpper();
        }
    }
}