using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Lamina.Views;

public sealed partial class CylinderCSAPage : Page
{
    public CylinderCSAPage()
    {
        InitializeComponent();
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(RadiusTextBox.Text, out double radius) &&
            double.TryParse(HeightTextBox.Text, out double height))
        {
            double csa = 2 * Math.PI * radius * height;
            ResultTextBlock.Text = $"CURVED SURFACE AREA: {csa:F2} sq. units".ToUpper();
        }
        else
        {
            ResultTextBlock.Text = "INVALID INPUT".ToUpper();
        }
    }
}