using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Lamina.Views;

public sealed partial class CubeSAPage : Page
{
    public CubeSAPage()
    {
        InitializeComponent();
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(SideTextBox.Text, out double side))
        {
            double surfaceArea = 6 * Math.Pow(side, 2);
            ResultTextBlock.Text = (surfaceArea.ToString("F2") + " sq. units").ToUpper();
        }
        else
        {
            ResultTextBlock.Text = "INVALID INPUT".ToUpper();
        }
    }
}