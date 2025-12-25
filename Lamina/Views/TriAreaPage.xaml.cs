using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Lamina.Views;

public sealed partial class TriAreaPage : Page
{
    public TriAreaPage()
    {
        InitializeComponent();
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(BaseTextBox.Text, out double triangleBase) &&
            double.TryParse(HeightTextBox.Text, out double height))
        {
            double area = 0.5 * triangleBase * height;
            ResultTextBlock.Text = $"{area:F2} sq. units".ToUpper();
        }
        else
        {
            ResultTextBlock.Text = "INVALID INPUT".ToUpper();
        }
    }
}