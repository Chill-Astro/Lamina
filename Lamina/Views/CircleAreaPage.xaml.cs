using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Lamina.Views;

public sealed partial class CircleAreaPage : Page
{
    public CircleAreaPage()
    {
        InitializeComponent();
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(RadiusTextBox.Text, out double radius))
        {
            double area = Math.PI * radius * radius;
            ResultTextBlock.Text = area.ToString("F2") + " sq.units"; ;
        }
        else
        {
            ResultTextBlock.Text = "INVALID INPUT";
        }
    }
}