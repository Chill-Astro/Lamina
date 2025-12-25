using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Lamina.Views;

public sealed partial class RhombusAreaPage : Page
{
    public RhombusAreaPage()
    {
        InitializeComponent();
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(Diagonal1TextBox.Text, out double diagonal1) &&
            double.TryParse(Diagonal2TextBox.Text, out double diagonal2))
        {
            double area = 0.5 * diagonal1 * diagonal2;
            ResultTextBlock.Text = area.ToString("F2") + " sq.units";
        }
        else
        {
            ResultTextBlock.Text = "INVALID INPUT";
        }
    }
}