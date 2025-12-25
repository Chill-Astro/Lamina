using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Lamina.Views;

public sealed partial class ConeCSAPage : Page
{
    public ConeCSAPage()
    {
        InitializeComponent();
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(RadiusTextBox.Text, out double radius) &&
            double.TryParse(SlantHeightTextBox.Text, out double slantHeight))
        {
            double csa = Math.PI * radius * slantHeight;
            ResultTextBlock.Text = $"CURVED SURFACE AREA: {csa:F2} sq. units".ToUpper();
        }
        else
        {
            ResultTextBlock.Text = "INVALID INPUT".ToUpper();
        }
    }
}