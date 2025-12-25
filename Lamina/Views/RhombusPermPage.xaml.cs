using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Lamina.Views;

public sealed partial class RhombusPermPage : Page
{
    public RhombusPermPage()
    {
        InitializeComponent();
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(SideTextBox.Text, out double side))
        {
            double perimeter = 4 * side;
            ResultTextBlock.Text = (perimeter.ToString("F2") + " units").ToUpper();
        }
        else
        {
            ResultTextBlock.Text = "INVALID INPUT".ToUpper();
        }
    }
}