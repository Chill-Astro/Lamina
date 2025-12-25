using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Lamina.Views;

public sealed partial class RectAreaPage : Page
{
    public RectAreaPage()
    {
        InitializeComponent();
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(BaseTextBox.Text, out double baseLength) &&
            double.TryParse(HeightTextBox.Text, out double height))
        {
            double area = baseLength * height;
            ResultTextBlock.Text = area.ToString("F2") + " sq.units";
        }
        else
        {
            ResultTextBlock.Text = "INVALID INPUT";
        }
    }
}