using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Lamina.Views;

public sealed partial class ITAreaPage : Page
{
    public ITAreaPage()
    {
        InitializeComponent();
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(EqualSideTextBox.Text, out double equalSide) &&
            double.TryParse(NonEqualSideTextBox.Text, out double nonEqualSide))
        {
            // Area formula: sqrt(4*a^2 - b^2) / 4
            double area = Math.Sqrt(4 * Math.Pow(equalSide, 2) - Math.Pow(nonEqualSide, 2)) / 4;
            ResultTextBlock.Text = area.ToString("F2") + " sq.units";
        }
        else
        {
            ResultTextBlock.Text = "INVALID INPUT";
        }
    }
}