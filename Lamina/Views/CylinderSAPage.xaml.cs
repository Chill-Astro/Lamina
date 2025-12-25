using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Lamina.Views;

public sealed partial class CylinderSAPage : Page
{
    public CylinderSAPage()
    {
        InitializeComponent();
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(RadiusTextBox.Text, out double radius) &&
            double.TryParse(HeightTextBox.Text, out double height))
        {
            double surfaceArea = 2 * Math.PI * radius * (radius + height);
            ResultTextBlock.Text = (surfaceArea.ToString("F2") + " sq. units").ToUpper();
        }
        else
        {
            ResultTextBlock.Text = "INVALID INPUT".ToUpper();
        }
    }
}