using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Lamina.Views;

public sealed partial class SphereCSAPage : Page
{
    public SphereCSAPage()
    {
        InitializeComponent();
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(RadiusTextBox.Text, out double radius))
        {
            double sa = 4 * Math.PI * Math.Pow(radius, 2);
            ResultTextBlock.Text = $"CURVED SURFACE AREA: {sa:F2} sq. units".ToUpper();
        }
        else
        {
            ResultTextBlock.Text = "INVALID INPUT".ToUpper();
        }
    }
}