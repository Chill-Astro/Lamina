using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Lamina.Views;

public sealed partial class CuboidVolumePage : Page
{
    public CuboidVolumePage()
    {
        InitializeComponent();
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(LengthTextBox.Text, out double length) &&
            double.TryParse(BreadthTextBox.Text, out double breadth) &&
            double.TryParse(HeightTextBox.Text, out double height))
        {
            double volume = length * breadth * height;
            ResultTextBlock.Text = (volume.ToString("F2") + " c. units").ToUpper();
        }
        else
        {
            ResultTextBlock.Text = "INVALID INPUT".ToUpper();
        }
    }
}