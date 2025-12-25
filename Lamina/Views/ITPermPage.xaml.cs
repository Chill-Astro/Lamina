using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Lamina.Views;

public sealed partial class ITPermPage : Page
{
    public ITPermPage()
    {
        InitializeComponent();
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(EqualSideTextBox.Text, out double equalSide) &&
            double.TryParse(NonEqualSideTextBox.Text, out double nonEqualSide))
        {
            double perimeter = 2 * equalSide + nonEqualSide;
            ResultTextBlock.Text = (perimeter.ToString("F2") + " units").ToUpper();
        }
        else
        {
            ResultTextBlock.Text = "INVALID INPUT".ToUpper();
        }
    }
}