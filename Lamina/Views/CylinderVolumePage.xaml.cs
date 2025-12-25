using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Lamina.Views;

public sealed partial class CylinderVolumePage : Page
{
    public CylinderVolumePage()
    {
        InitializeComponent();
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(RadiusTextBox.Text, out double radius) &&
            double.TryParse(HeightTextBox.Text, out double height))
        {
            double volume = Math.PI * Math.Pow(radius, 2) * height;
            ResultTextBlock.Text = (volume.ToString("F2") + " c. units").ToUpper();
        }
        else
        {
            ResultTextBlock.Text = "INVALID INPUT".ToUpper();
        }
    }
}