using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Lamina.Views;

public sealed partial class SpherePage : Page
{
    public SpherePage()
    {
        InitializeComponent();
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(RadiusTextBox.Text, out double radius))
        {
            double volume = (4.0 / 3.0) * Math.PI * Math.Pow(radius, 3);
            ResultTextBlock.Text = (volume.ToString("F2") + " c. units").ToUpper();
        }
        else
        {
            ResultTextBlock.Text = "INVALID INPUT".ToUpper();
        }
    }
}