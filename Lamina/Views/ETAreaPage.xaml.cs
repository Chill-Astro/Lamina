using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Lamina.Views;

public sealed partial class ETAreaPage : Page
{
    public ETAreaPage()
    {
        InitializeComponent();
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(SideTextBox.Text, out double side))
        {
            double area = (Math.Sqrt(3) / 4) * side * side;
            ResultTextBlock.Text = area.ToString("F2") + " sq.units";
        }
        else
        {
            ResultTextBlock.Text = "INVALID INPUT";
        }
    }
}