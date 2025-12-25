using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Lamina.Views;

public sealed partial class SCircleAreaPage : Page
{
    public SCircleAreaPage()
    {
        InitializeComponent();
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(RadiusTextBox.Text, out double radius))
        {
            double area = 0.5 * Math.PI * radius * radius;
            ResultTextBlock.Text = area.ToString("F2") + " sq.units";
        }
        else
        {
            ResultTextBlock.Text = "INVALID INPUT";
        }
    }
}